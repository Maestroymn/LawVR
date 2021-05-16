# coding=utf-8
import speech_recognition as sr
import time
import os
from datetime import datetime
import wave
import contextlib
import sys

lan = sys.argv[1]

dir = os.path.dirname(__file__)
dir += "/Speeches/speech.wav"
audioFileExists = False
r = sr.Recognizer()

def GetDuration(dir):
    with contextlib.closing(wave.open(dir, 'r')) as f:
        frames = f.getnframes()
        rate = f.getframerate()
        duration = frames / float(rate)
        return duration

try:
    with sr.AudioFile(dir) as source:
        print("listening from file")
        speechStartTime = datetime.now()
        audio = r.record(source)
        audioFileExists = True
        try:
            text = r.recognize_google(audio, language=lan)
            print("Speech*" + text.encode('utf-8').strip())  # encode edilmezse türkçe karakterleri texte basarken patlıyor
            print("StartTime*" + str(speechStartTime))
            print("Duration*" + str(GetDuration(dir)))

        except sr.RequestError:
            print("Something went wrong!")
        except sr.UnknownValueError:
            print("Couldn't clearly understand?!")
        except Exception as e:
            print(str(e))
            print("change python")
    os.remove(dir)

except IOError:
    print("listening for new")
except WindowsError:
    print("cannot delete")
except:
    print("this is undefined exception in opening file")


if not audioFileExists:
    r.pause_threshold = 5
    with sr.Microphone() as source:
        time.sleep(1)
        r.adjust_for_ambient_noise(source, duration=0.5)

        try:
            speechStartTime = datetime.now()
            audio = r.listen(source, timeout=5)
            with open(dir,"wb") as f:
                f.write(audio.get_wav_data())


            text = r.recognize_google(audio, language=lan)
            print("Speech*"+text.encode('utf-8').strip())  # encode edilmezse türkçe karakterleri texte basarken patlıyor
            print("StartTime*" + str(speechStartTime))
            print("Duration*" + str(GetDuration(dir)))
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


