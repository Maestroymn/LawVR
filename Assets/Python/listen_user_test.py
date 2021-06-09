import speech_recognition as sr
import time
import os
from datetime import datetime
import wave
import contextlib
import sys
import traceback
import DBComm as dbc

lan = sys.argv[1]
SessionID = sys.argv[2]
UserName = sys.argv[3]
UserRole = sys.argv[4]

ErrorMessage = "THIS SPEECH COULDN'T BE RECORDED PROPERLY! MISSING LOG!"


speech_directory = os.getcwd()
print(speech_directory)
speech_directory += "/Speeches/speech.wav"
print(speech_directory)
audioFileExists = False
r = sr.Recognizer()

def GetDuration(speech_directory):
    with contextlib.closing(wave.open(speech_directory, 'r')) as f:
        frames = f.getnframes()
        rate = f.getframerate()
        duration = frames / float(rate)
        return duration
dbc.connect()
try:
    with sr.AudioFile(speech_directory) as source:
        print("listening from file")
        speechStartTime = datetime.now()
        audio = r.record(source)
        audioFileExists = True
        try:
            text = r.recognize_google(audio, language=lan)
            speech_duration = GetDuration(speech_directory)
            dbc.uploadSpeechLog(SessionID,UserName,UserRole,str(text),str(speechStartTime),speech_duration)
        except sr.RequestError:
            dbc.uploadSpeechLog(SessionID, UserName, UserRole, ErrorMessage, datetime.now(), 0.0)
            print("Something went wrong!")
        except sr.UnknownValueError:
            dbc.uploadSpeechLog(SessionID, UserName, UserRole, ErrorMessage, datetime.now(), 0.0)
            print("Couldn't clearly understand?!")
        except Exception as e:
            dbc.uploadSpeechLog(SessionID, UserName, UserRole, ErrorMessage, datetime.now(), 0.0)
            print(str(e))
    os.remove(speech_directory)

except IOError:
    print("listening for new")
except WindowsError:
    print("cannot delete")
except:
    print("this is undefined exception in opening file")


if not audioFileExists:
    r.pause_threshold = 3
    with sr.Microphone() as source:
        time.sleep(1)
        r.adjust_for_ambient_noise(source, duration=0.5)

        try:
            speechStartTime = datetime.now()
            audio = r.listen(source, timeout=5)
            with open(speech_directory,"wb") as f:
                f.write(audio.get_wav_data())

            text = r.recognize_google(audio, language=lan)
            speech_duration = GetDuration(speech_directory)
            dbc.uploadSpeechLog(SessionID, UserName, UserRole, str(text), str(speechStartTime), speech_duration)
        except sr.RequestError:
            dbc.uploadSpeechLog(SessionID, UserName, UserRole, ErrorMessage, datetime.now(), 0.0)
            print("Something went wrong!")
        except sr.UnknownValueError:
            dbc.uploadSpeechLog(SessionID, UserName, UserRole, ErrorMessage, datetime.now(), 0.0)
            print("Couldn't clearly understand?!")
        except sr.WaitTimeoutError:
            dbc.uploadSpeechLog(SessionID, UserName, UserRole, ErrorMessage, datetime.now(), 0.0)
            print("Can't hear you")
        except Exception as e:
            dbc.uploadSpeechLog(SessionID, UserName, UserRole, ErrorMessage, datetime.now(), 0.0)
            print(traceback.format_exc())

        try:
            os.remove(speech_directory)
        except Exception as e:
            print("couldn't delete")
dbc.disconnect()

