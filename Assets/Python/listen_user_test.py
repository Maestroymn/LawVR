# coding=utf-8
import speech_recognition as sr
import time

print("a")
r = sr.Recognizer()


loop = 1
while loop:
    r = sr.Recognizer()
    print("recognized")
    r.pause_threshold = 2
    with sr.Microphone() as source:
        time.sleep(1)
        print("adjus")
        r.adjust_for_ambient_noise(source, duration=0.5)
        print("ted")
        audio_text = r.listen(source, timeout=5)
        try:
            print("enough")
            text = r.recognize_google(audio_text, language="tr-TR")
            if "yeter" in text:
                loop = 0



            print(text.encode('utf-8').strip()+"\n")  # encode edilmezse türkçe karakterleri texte basarken patlıyor
        except sr.RequestError:
            print("Something went wrong!")
        except sr.UnknownValueError:
            print("Couldn't clearly understand?!")
