using UnityEngine;
using TMPro;

/// <summary>
/// Populates the About panel with det,vkbZled game info for OptiMind (multi-correct logic game).
/// </summary>
public class AboutGameManager : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Assign the TMP_Text component for the About section.")]
    public TMP_Text aboutText;

    void Start()
    {
        if (aboutText == null)
        {
            Debug.LogError("AboutGameManager: 'aboutText' reference is missing.");
            return;
        }

        // aboutText.text =
        //     "ekbaMeSi ,vkbZ ,d rst+&rjkZj] fganh&Hkk\"kk 'kSf{kd [ksy gS tgka f[kykM+h —f=e cqf)eÙkk ds rdZ esa xksrk yxkrs gSaA fV~oLV\ vki ladsr ugha fy[krs gSa & vki mUgsa fjolZ bathfu;j djrs gSa çR;sd nkSj] [ksy ,d ,vkbZ&tfur vkmViqV fn[kkrk gSA f[kykM+h dh pqukSrh ;g vuqeku yxkus ds fy, gS fd fdl rjg dh çfrfØ;k lcls vfèkd laHkkouk gS&rhu leku&lkmafMax fodYiksa ls pquukA ;g le; ds f[kykQ ,d nkSM+ gS] u dsoy Rofjr Økf¶Vax dh vkidh le> dk ijh{k.k djsa] cfYd ,vkbZ dh rjg lkspus dh vkidh {kerk HkhA ,vkbZ@,e,y voèkkj.kkvksa vkSj \'kh?kz bathfu;fjax ewy ckrsa lh[kus okys Nk=ksa ds fy, vkn\'kZ & ,d etsnkj] lgt rjhds lsA";
    }
}
