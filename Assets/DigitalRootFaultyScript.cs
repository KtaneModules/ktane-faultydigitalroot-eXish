using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System;
using System.Text.RegularExpressions;

public class DigitalRootFaultyScript : MonoBehaviour {

    public KMAudio audio;
    public KMBombInfo bomb;

    public KMSelectable[] buttons;

    public GameObject numDisplay1;
    public GameObject numDisplay2;

    public GameObject scrDisplay1;
    public GameObject scrDisplay2;
    public GameObject scrDisplay3;

    public GameObject inputs;
    public Material[] ledmats;

    private int topTotal;

    private int ansTop;
    private int bottomNum;
    private int type;

    private Coroutine displayCrack;

    private string binaryAns;
    private string madeAns;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        moduleSolved = false;
        foreach(KMSelectable obj in buttons){
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
        }
    }

    void Start () {
        inputs.GetComponent<MeshRenderer>().material = ledmats[0];
        topTotal = 0;
        ansTop = 0;
        bottomNum = 0;
        type = 0;
        binaryAns = "";
        madeAns = "";
        generateNumsAndTotals();
        calcAnswer();
        displayCrack = StartCoroutine(brokeDisplay());
    }

    void PressButton(KMSelectable pressed)
    {
        if(moduleSolved != true)
        {
            pressed.AddInteractionPunch(0.25f);
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            inputs.GetComponent<MeshRenderer>().material = ledmats[1];
            if(pressed == buttons[0])
            {
                madeAns += "1";
            }else if (pressed == buttons[1])
            {
                madeAns += "0";
            }
            if (madeAns.Length == 4)
            {
                if (madeAns.Equals(binaryAns))
                {
                    Debug.LogFormat("[Faulty Digital Root #{0}] Input of '{1}' was correct! Module Disarmed!", moduleId, madeAns);
                    StopCoroutine(displayCrack);
                    displayCrack = StartCoroutine(victory());
                }
                else
                {
                    Debug.LogFormat("[Faulty Digital Root #{0}] Input of '{1}' was incorrect! Resetting module...", moduleId, madeAns);
                    StopCoroutine(displayCrack);
                    bomb.GetComponent<KMBombModule>().HandleStrike();
                    numDisplay1.GetComponent<TextMesh>().text = "";
                    numDisplay2.GetComponent<TextMesh>().text = "";
                    Start();
                }
            }
        }
    }

    private void generateNumsAndTotals()
    {
        string nums1 = "[Faulty Digital Root #{0}] Numbers from top 3 displays from left to right: ";
        int rand = UnityEngine.Random.Range(0, 10);
        numDisplay1.GetComponent<TextMesh>().text = "" + rand;
        numDisplay2.GetComponent<TextMesh>().text = "" + rand;
        bottomNum = rand;
        type = bottomNum % 2;
        Debug.LogFormat("[Faulty Digital Root #{0}] The broken display shows a {1}, which mod 2 is {2}", moduleId, bottomNum, type);
        if(type == 0)
        {
            Debug.LogFormat("[Faulty Digital Root #{0}] The type of digital root to take for the top 3 numbers is an Additive Digital Root", moduleId);
            for (int i = 0; i < 3; i++)
            {
                int dec = UnityEngine.Random.Range(0, 10);
                switch (i)
                {
                    case 0: scrDisplay1.GetComponent<TextMesh>().text = "" + dec; nums1 += (dec + " "); topTotal += dec; break;
                    case 1: scrDisplay2.GetComponent<TextMesh>().text = "" + dec; nums1 += (dec + " "); topTotal += dec; break;
                    case 2: scrDisplay3.GetComponent<TextMesh>().text = "" + dec; nums1 += (dec + " "); topTotal += dec; break;
                }
            }
        }
        else
        {
            Debug.LogFormat("[Faulty Digital Root #{0}] The type of digital root to take for the top 3 numbers is a Multiplicative Digital Root", moduleId);
            topTotal = 1;
            for (int i = 0; i < 3; i++)
            {
                int dec = UnityEngine.Random.Range(0, 10);
                switch (i)
                {
                    case 0: scrDisplay1.GetComponent<TextMesh>().text = "" + dec; nums1 += (dec + " "); topTotal *= dec; break;
                    case 1: scrDisplay2.GetComponent<TextMesh>().text = "" + dec; nums1 += (dec + " "); topTotal *= dec; break;
                    case 2: scrDisplay3.GetComponent<TextMesh>().text = "" + dec; nums1 += (dec + " "); topTotal *= dec; break;
                }
            }
        }
        Debug.LogFormat(nums1, moduleId);
    }

    private int digitalRoot(int dig)
    {
        string combo = "" + dig;
        while (combo.Length > 1)
        {
            int total = 0;
            for (int i = 0; i < combo.Length; i++)
            {
                int temp = 0;
                int.TryParse(combo.Substring(i, 1), out temp);
                total += temp;
            }
            combo = total + "";
        }
        int temp2 = 0;
        int.TryParse(combo, out temp2);
        return temp2;
    }
    private int digitalRoot2(int dig)
    {
        string combo = "" + dig;
        while (combo.Length > 1)
        {
            int total = 1;
            for (int i = 0; i < combo.Length; i++)
            {
                int temp = 0;
                int.TryParse(combo.Substring(i, 1), out temp);
                total *= temp;
            }
            combo = total + "";
        }
        int temp2 = 0;
        int.TryParse(combo, out temp2);
        return temp2;
    }

    private void calcAnswer()
    {
        if(type == 0)
        {
            ansTop = digitalRoot(topTotal);
        }
        else
        {
            ansTop = digitalRoot2(topTotal);
        }
        if (ansTop == 0)
        {
            binaryAns = "0000";
        }
        else if (ansTop == 1)
        {
            binaryAns = "0001";
        }
        else if (ansTop == 2)
        {
            binaryAns = "0010";
        }
        else if (ansTop == 3)
        {
            binaryAns = "0011";
        }
        else if (ansTop == 4)
        {
            binaryAns = "0100";
        }
        else if (ansTop == 5)
        {
            binaryAns = "0101";
        }
        else if (ansTop == 6)
        {
            binaryAns = "0110";
        }
        else if (ansTop == 7)
        {
            binaryAns = "0111";
        }
        else if (ansTop == 8)
        {
            binaryAns = "1000";
        }
        else if (ansTop == 9)
        {
            binaryAns = "1001";
        }
        Debug.LogFormat("[Faulty Digital Root #{0}] The answer after taking the digital root is {1}, which in binary is {2}", moduleId, ansTop, binaryAns);
    }

    private IEnumerator brokeDisplay()
    {
        yield return null;
        while (true)
        {
            float rand = UnityEngine.Random.Range(0.1f,1.0f);
            float rand2 = UnityEngine.Random.Range(0.75f, 5.0f);
            numDisplay1.GetComponent<TextMesh>().text = "";
            numDisplay2.GetComponent<TextMesh>().text = "";
            yield return new WaitForSeconds(rand);
            numDisplay1.GetComponent<TextMesh>().text = ""+bottomNum;
            numDisplay2.GetComponent<TextMesh>().text = ""+bottomNum;
            yield return new WaitForSeconds(rand2);
        }
    }

    private IEnumerator victory()
    {
        yield return null;
        moduleSolved = true;
        numDisplay1.GetComponent<TextMesh>().text = "";
        numDisplay2.GetComponent<TextMesh>().text = "";
        for (int i = 0; i < 130; i++)
        {
            int rand1 = UnityEngine.Random.RandomRange(0, 10);
            int rand2 = UnityEngine.Random.RandomRange(0, 10);
            int rand3 = UnityEngine.Random.RandomRange(0, 10);
            if (i < 50)
            {
                scrDisplay1.GetComponent<TextMesh>().text = rand1 + "";
                scrDisplay2.GetComponent<TextMesh>().text = rand2 + "";
                scrDisplay3.GetComponent<TextMesh>().text = rand3 + "";
            }
            else if(i < 80)
            {
                scrDisplay1.GetComponent<TextMesh>().text = "G";
                scrDisplay2.GetComponent<TextMesh>().text = rand2 + "";
                scrDisplay3.GetComponent<TextMesh>().text = rand3 + "";
            }
            else if (i < 100)
            {
                scrDisplay1.GetComponent<TextMesh>().text = "G";
                scrDisplay2.GetComponent<TextMesh>().text = "G";
                scrDisplay3.GetComponent<TextMesh>().text = rand3 + "";
            }
            else if (i < 130)
            {
                scrDisplay1.GetComponent<TextMesh>().text = "G";
                scrDisplay2.GetComponent<TextMesh>().text = "G";
                scrDisplay3.GetComponent<TextMesh>().text = "!";
            }
            yield return new WaitForSeconds(0.025f);
        }
        bomb.GetComponent<KMBombModule>().HandlePass();
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
        StartCoroutine(brokeDisplay2());
        StopCoroutine(displayCrack);
    }

    private IEnumerator brokeDisplay2()
    {
        yield return null;
        while (true)
        {
            float rand = UnityEngine.Random.Range(0.1f, 1.0f);
            float rand2 = UnityEngine.Random.Range(0.75f, 5.0f);
            numDisplay1.GetComponent<TextMesh>().text = "";
            numDisplay2.GetComponent<TextMesh>().text = "";
            yield return new WaitForSeconds(rand);
            numDisplay1.GetComponent<TextMesh>().text = "☺";
            numDisplay2.GetComponent<TextMesh>().text = "☺";
            yield return new WaitForSeconds(rand2);
        }
    }

    //twitch plays
    private bool cmdIsValid(string param)
    {
        string[] parameters = param.Split(' ',',');
        for(int i = 1; i < parameters.Length; i++)
        {
            if(!parameters[i].EqualsIgnoreCase("yes") && !parameters[i].EqualsIgnoreCase("no"))
            {
                return false;
            }
        }
        return true;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press yes [Presses the yes button] | !{0} press no [Presses the no button] | !{0} reset [Resets all inputs] | Yes's and No's can be chained";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*reset\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            madeAns = "";
            Debug.LogFormat("[Faulty Digital Root #{0}] Reset of inputs triggered! (TP)", moduleId);
            inputs.GetComponent<MeshRenderer>().material = ledmats[0];
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if(parameters.Length > 1)
            {
                if (cmdIsValid(command))
                {
                    yield return null;
                    for (int i = 1; i < parameters.Length; i++)
                    {
                        if (parameters[i].EqualsIgnoreCase("yes"))
                        {
                            buttons[0].OnInteract();
                        }else if (parameters[i].EqualsIgnoreCase("no"))
                        {
                            buttons[1].OnInteract();
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            yield break;
        }
    }
}
