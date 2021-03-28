# coding=utf-8
import speech_recognition as sr
import time
print("selam")
f = open("C:\\Users\\Deniz Bayan\\Desktop\\try.txt", "w")

r = sr.Recognizer()

loop = 1
print("selam")
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

            print("aselam")
            f.write(text.encode('utf-8').strip()+"\n")  # encode edilmezse türkçe karakterleri texte basarken patlıyor
            if "yeter" in text:
                f.close()

        except sr.RequestError:

            f.write("hatataaaaaa")
        except sr.UnknownValueError:

            f.write("anlamadim?????")
