# coding=utf-8
import speech_recognition as sr
import time

r = sr.Recognizer()

loop = 1
while loop:
    r = sr.Recognizer()
    #r.pause_threshold = 5
    with sr.Microphone() as source:
        time.sleep(1)
        r.adjust_for_ambient_noise(source)

        audio_text = r.listen(source)
        try:
            text = r.recognize_google(audio_text, language="tr-TR")
            if "yeter" in text:
                loop = 0
            print(text.encode('utf-8').strip()+"\n")  # encode edilmezse türkçe karakterleri texte basarken patlıyor
        except sr.RequestError:
            print("Something went wrong!")
        except sr.UnknownValueError:
            print("Couldn't clearly understand?!")
