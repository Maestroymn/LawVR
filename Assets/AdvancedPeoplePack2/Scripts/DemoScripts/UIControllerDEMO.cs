﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AdvancedCustomizableSystem;
using Data;

public class UIControllerDEMO : MonoBehaviour
{
    public CharacterCustomization CharacterCustomization;
    [Space(15)]

    public Text playbutton_text;

    public Text panelNameText;

    public Slider fatSlider;
    public Slider musclesSlider;
    public Slider thinSlider;

    public Slider slimnessSlider;
    public Slider breastSlider;

    public Slider heightSlider;

    public Slider headSizeSlider;

    public Slider headOffsetSlider;

    public Slider[] faceShapeSliders;

    public RectTransform HairPanel;
    public RectTransform BeardPanel;
    public RectTransform ShirtPanel;
    public RectTransform PantsPanel;
    public RectTransform ShoesPanel;
    public RectTransform HatPanel;
    public RectTransform AccessoryPanel;

    public RectTransform FaceEditPanel;
    public RectTransform BaseEditPanel;

    public RectTransform SkinColorPanel;
    public RectTransform EyeColorPanel;
    public RectTransform HairColorPanel;
    public RectTransform UnderpantsColorPanel;

    public RectTransform EmotionsPanel;

    public Image SkinColorButtonColor;
    public Image EyeColorButtonColor;
    public Image HairColorButtonColor;
    public Image UnderpantsColorButtonColor;

    public Vector3[] CameraPositionForPanels;
    public Vector3[] CameraEulerForPanels;
    int currentPanelIndex = 0;

    public Camera Camera;

    #region ButtonEvents

    public void ShowFaceEdit()
    {
        FaceEditPanel.gameObject.SetActive(true);
        BaseEditPanel.gameObject.SetActive(false);
        currentPanelIndex = 1;
        panelNameText.text = "FACE CUSTOMIZER";
    }

    public void ShowBaseEdit()
    {
        FaceEditPanel.gameObject.SetActive(false);
        BaseEditPanel.gameObject.SetActive(true);
        currentPanelIndex = 0;
        panelNameText.text = "BASE CUSTOMIZER";
    }

    public void SetFaceShape(int index)
    {
        CharacterCustomization.SetFaceShape((FaceShapeType)(index), faceShapeSliders[index].value);
    }

    public void SetHeadOffset()
    {
        CharacterCustomization.SetHeadOffset(headOffsetSlider.value);
    }

    public void BodyFat()
    {
        CharacterCustomization.SetBodyShape(BodyShapeType.Fat, fatSlider.value);
    }
    public void BodyMuscles()
    {
        CharacterCustomization.SetBodyShape(BodyShapeType.Muscles, musclesSlider.value);
    }
    public void BodyThin()
    {
        CharacterCustomization.SetBodyShape(BodyShapeType.Thin, thinSlider.value);
    }

    public void BodySlimness()
    {
        CharacterCustomization.SetBodyShape(BodyShapeType.Slimness, slimnessSlider.value);
    }
    public void BodyBreast()
    {
        CharacterCustomization.SetBodyShape(BodyShapeType.BreastSize, breastSlider.value,
            new string[] { "Chest", "Stomach", "Head" },
            new ClothesPartType[] { ClothesPartType.Shirt }
            );
    }
    public void SetHeight()
    {
        CharacterCustomization.SetHeight(heightSlider.value);
    }
    public void SetHeadSize()
    {
        CharacterCustomization.SetHeadSize(headSizeSlider.value);
    }
    public void SetNewSkinColor(Color color)
    {
        SkinColorButtonColor.color = color;
        CharacterCustomization.SetBodyColor(BodyColorPart.Skin, color);
    }
    public void SetNewEyeColor(Color color)
    {
        EyeColorButtonColor.color = color;
        CharacterCustomization.SetBodyColor(BodyColorPart.Eye, color);
    }
    public void SetNewHairColor(Color color)
    {
        HairColorButtonColor.color = color;
        CharacterCustomization.SetBodyColor(BodyColorPart.Hair, color);
    }
    public void SetNewUnderpantsColor(Color color)
    {
        UnderpantsColorButtonColor.color = color;
        CharacterCustomization.SetBodyColor(BodyColorPart.Underpants, color);
    }
    public void VisibleSkinColorPanel(bool v)
    {
        HideAllPanels();
        SkinColorPanel.gameObject.SetActive(v);
    }
    public void VisibleEyeColorPanel(bool v)
    {
        HideAllPanels();
        EyeColorPanel.gameObject.SetActive(v);
    }
    public void VisibleHairColorPanel(bool v)
    {
        HideAllPanels();
        HairColorPanel.gameObject.SetActive(v);
    }
    public void VisibleUnderpantsColorPanel(bool v)
    {
        HideAllPanels();
        UnderpantsColorPanel.gameObject.SetActive(v);
    }
    public void ShirtPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            ShirtPanel.gameObject.SetActive(false);
        else
            ShirtPanel.gameObject.SetActive(true);
    }
    public void PantsPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            PantsPanel.gameObject.SetActive(false);
        else
            PantsPanel.gameObject.SetActive(true);
    }
    public void ShoesPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            ShoesPanel.gameObject.SetActive(false);
        else
            ShoesPanel.gameObject.SetActive(true);
    }
    public void HairPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            HairPanel.gameObject.SetActive(false);
        else
            HairPanel.gameObject.SetActive(true);

        currentPanelIndex = (v) ? 1 : 0;
    }
    public void BeardPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            BeardPanel.gameObject.SetActive(false);
        else
            BeardPanel.gameObject.SetActive(true);

        currentPanelIndex = (v) ? 1 : 0;
    }
    public void HatPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            HatPanel.gameObject.SetActive(false);
        else
            HatPanel.gameObject.SetActive(true);
        currentPanelIndex = (v) ? 1 : 0;
    }
    public void EmotionsPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            EmotionsPanel.gameObject.SetActive(false);
        else
            EmotionsPanel.gameObject.SetActive(true);
        currentPanelIndex = (v) ? 1 : 0;
    }
    public void AccessoryPanel_Select(bool v)
    {
        HideAllPanels();
        if (!v)
            AccessoryPanel.gameObject.SetActive(false);
        else
            AccessoryPanel.gameObject.SetActive(true);
        currentPanelIndex = (v) ? 1 : 0;
    }
    public void EmotionsChange_Event(int index)
    {
        var emotion = CharacterCustomization.emotionPresets[index];
        if (emotion != null)
            CharacterCustomization.PlayEmotion(emotion.name, 2f);
    }
    public void HairChange_Event(int index)
    {
        CharacterCustomization.SetHairByIndex(index);
    }
    public void BeardChange_Event(int index)
    {
        CharacterCustomization.SetBeardByIndex(index);
    }
    public void ShirtChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(ClothesPartType.Shirt, index);
    }
    public void PantsChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(ClothesPartType.Pants, index);
    }
    public void ShoesChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(ClothesPartType.Shoes, index);
    }
    public void HatChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(ClothesPartType.Hat, index);
    }
    public void AccessoryChange_Event(int index)
    {
        CharacterCustomization.SetElementByIndex(ClothesPartType.Accessory, index);
    }
    public void HideAllPanels()
    {
        SkinColorPanel.gameObject.SetActive(false);
        EyeColorPanel.gameObject.SetActive(false);
        HairColorPanel.gameObject.SetActive(false);
        UnderpantsColorPanel.gameObject.SetActive(false);
        if (EmotionsPanel != null)
            EmotionsPanel.gameObject.SetActive(false);
        if (BeardPanel != null)
            BeardPanel.gameObject.SetActive(false);
        HairPanel.gameObject.SetActive(false);
        ShirtPanel.gameObject.SetActive(false);
        PantsPanel.gameObject.SetActive(false);
        ShoesPanel.gameObject.SetActive(false);
        HatPanel.gameObject.SetActive(false);
        AccessoryPanel.gameObject.SetActive(false);

        currentPanelIndex = 0;
    }
    public void SaveToFile()
    {
        CharacterCustomization.SaveToFile();
    }
    public void LoadFromFile()
    {
        CharacterCustomization.LoadFromFile();
    }
    public void ClearFromFile()
    {
        CharacterCustomization.ClearFromFile();
        CharacterCustomization.ResetAll();
    }
    public void Randimize()
    {
        CharacterCustomization.Randomize();
    }
    bool walk_active = false;

    public void PlayAnim()
    {
        walk_active = !walk_active;

        foreach (Animator a in CharacterCustomization.GetAnimators())
            a.SetBool("walk", walk_active);

        playbutton_text.text = (walk_active) ? "STOP" : "PLAY";
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(DataKeyValues.__MAIN_UI_SCENE__);
    }
    #endregion

}
