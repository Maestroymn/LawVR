# coding=utf-8
import speech_recognition as sr
import time
import os
from datetime import datetime

dir = os.path.dirname(__file__)
dir += "/Speeches/speech.wav"
audioFileExists = False
r = sr.Recognizer()

try:
    with sr.AudioFile(dir) as source:
        print("listening from file")
        speechStartTime = datetime.now()
        startTime = time.time()
        audio = r.record(source)
        endTime = time.time()
        audioFileExists = True
        try:
            text = r.recognize_google(audio, language="tr-TR")
            print("Speech*" + text.encode('utf-8').strip())  # encode edilmezse türkçe karakterleri texte basarken patlıyor
            print("StartTime*" + str(speechStartTime))
            print("Duration*" + str(endTime - startTime))

        except sr.RequestError:
            print("Something went wrong!")
        except sr.UnknownValueError:
            print("Couldn't clearly understand?!")
    os.remove(dir)

except IOError:
    print("listening for new")
except WindowsError:
    print("cannot delete")

if not audioFileExists:
    r.pause_threshold = 5
    with sr.Microphone() as source:
        time.sleep(1)
        r.adjust_for_ambient_noise(source, duration=0.5)

        try:
            speechStartTime = datetime.now()
            startTime = time.time()
            audio = r.listen(source, timeout=5, )
            endTime = time.time()
            with open(dir,"wb") as f:
                f.write(audio.get_wav_data())


            text = r.recognize_google(audio, language="tr-TR")
            print("Speech*"+text.encode('utf-8').strip())  # encode edilmezse türkçe karakterleri texte basarken patlıyor
            print("StartTime*" + str(speechStartTime))
            print("Duration*" + str(endTime - startTime))
            os.remove(dir)
        except sr.RequestError:
            print("Something went wrong!")
        except sr.UnknownValueError:
            print("Couldn't clearly understand?!")
        except sr.WaitTimeoutError:
            print("Can't hear you")
        except Exception as e:
            print(str(e))
            print("change python")


