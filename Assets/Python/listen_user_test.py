import speech_recognition as sr
import UnityEngine as ue
import time

ue.Debug.Log("recognize initialize")
r = sr.Recognizer()
ue.Debug.Log("recognized")
loop = 1

while loop:
    r = sr.Recognizer()
    r.pause_threshold = 5
    with sr.Microphone() as source:
        time.sleep(1)
        r.adjust_for_ambient_noise(source)


        ue.Debug.Log("listening")
        audio_text = r.listen(source)
        ue.Debug.Log(type(audio_text))

        try:
            text = r.recognize_google(audio_text, language="tr-TR")
            if "yeter" in text:
                loop = 0
            ue.Debug.Log("did you mean " + text)
        except sr.RequestError:
            ue.Debug.Log("hatataaaaaa")
        except sr.UnknownValueError:
            ue.Debug.Log("anlamadim?????")



