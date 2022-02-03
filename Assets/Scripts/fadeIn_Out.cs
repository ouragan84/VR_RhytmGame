using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ExampleDelegate (int parameter);

public class fadeIn_Out : MonoBehaviour
{
    private Animator anim;
    private int param;
    private ExampleDelegate funcToCall;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        //SomeHandler (MyTarget);
    }

    public void Fade(){
        funcToCall = nothing;
        anim.Play("Base Layer.FadeIn&Out", -1);
    }

    public void FadeWithFunctionCall(ExampleDelegate theDelegat, int param){
        funcToCall = theDelegat;
        this.param = param;
        anim.Play("Base Layer.FadeIn&Out", -1);
    }

    public void nothing(int p){

    }

    public void MidFade(){
        funcToCall(param);
    }
}
