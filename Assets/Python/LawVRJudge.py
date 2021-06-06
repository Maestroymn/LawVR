import sys
import math
import TextManager as tm
import DBComm as dbc


# train = tm.createDataset()
train = {'target': [], 'data': []}
case_role = ""

# Tr2Eng = str.maketrans("çğıöşü", "cgiosu")
# name = 'Full Name Here'
# name_eng = name.translate(Tr2Eng)


def main():
    global case_role

    sessionID = sys.argv[1]
    case_role = sys.argv[2]
    username = sys.argv[3]
    dbc.connect()
    text = dbc.getSessionScript(sessionID)
    individual_text = dbc.getSessionSpeakerScript(sessionID, case_role)
    dbc.disconnect()
    giveFeedback(username, sessionID, case_role, text, individual_text)


def preProcessing(text):
    import re
    import string
    # Convert text to lowercase
    outText = text.lower()

    # Remove numbers
    outText = re.sub(r'\d+', '', outText)

    # Remove punctuation
    outText = outText.translate(str.maketrans("", "", string.punctuation))

    # Remove whitespaces
    outText = outText.strip()

    # Remove stopwords
    from nltk.corpus import stopwords
    from nltk.tokenize import word_tokenize


    stop_words = set(stopwords.words('turkish'))

    tokens = word_tokenize(outText)
    outText = [i for i in tokens if not i in stop_words]

    turkish_names = []
    with open("resources/names.txt", 'r', encoding='utf8') as read_obj:
        for line in read_obj:
            turkish_names.append(line.strip().lower())
    name_list = set(turkish_names)

    turkish_surnames = []
    with open("resources/surnames.txt", 'r', encoding='utf8') as read_obj:
        for line in read_obj:
            turkish_surnames.append(line.strip().lower())
    surname_list = set(turkish_surnames)

    # Addition to the stopwords, remove the Turkish names and surnames as well.
    outText = [i for i in outText if not i in name_list]
    outText = [i for i in outText if not i in surname_list]

    # Lemmatization
    from nltk.stem import WordNetLemmatizer
    lemmatizer = WordNetLemmatizer()
    result = []
    for word in outText:
        result.append(lemmatizer.lemmatize(word))

    return result


def constructDictionary():
    dictionary = dict()
    sortedDictionary = dict()
    for i in range(len(train.data)):
        words = preProcessing(train.data[i])
        classOfWords = train.target[i]
        for j in range(len(words)):
            if words[j] in dictionary:
                dictionary[words[j]][classOfWords] += 1
                dictionary[words[j]][-1] += 1 # Increasing the last value of the array,
                # which is the total number of usage of a word
            else:
                key = words[j]
                value = [0] * 3
                value[classOfWords] = 1
                value[-1] = 1
                dictionary[key] = value

    # k = 0
    for x in sorted(dictionary, key=lambda x: dictionary[x][2], reverse=True):
        sortedDictionary[x] = dictionary[x]
        # k += 1
        # if k == 5000:
        #     break

    return sortedDictionary


# This function gives the prior probabilities (probabilities of each classes)
# returns an array of 2: one probability for every class
# for probability of one class: (No. of documents classified into the category) / (Total number of documents)
def calcPriors():
    # probList = [train.target.count(0) / len(train.data),
    #             train.target.count(1) / len(train.data)]
    probList = [0.5, 0.5]
    return probList


def findProbabilities(dictionary, text):
    priors = calcPriors()
    probabilities = [0.0] * 2
    for word in text:
        if word in dictionary:
            for x in range(0, 2):  # find the probabilities for both classes for the current word
                # To apply Laplace Smoothing, add 1 to numerator and add the size of the vocabulary to denominator
                # (Number of occurrence of the word in all the documents from a category+1) divided by
                # (All the occurrences of the word in every category + size of dictionary)
                probabilities[x] += math.log((dictionary[word][x] + 1) / (dictionary[word][-1] + len(dictionary)), 10)

    for x in range(0, 2):
        priors[x] = math.log((priors[x]), 10)

    for i in range(0, 2):
        probabilities[i] += priors[i]

    indexOfMax = probabilities.index(max(probabilities))

    return indexOfMax


def updateDatabase(prediction, keyword_list):
    new_list = []

    # Assign new values based on the result of the case (ML prediction)
    if prediction == 0:
        # lose
        for kw in keyword_list:
            new_list.append((kw[0], -(1 - kw[1])))

    elif prediction == 1:
        # win
        for kw in keyword_list:
            new_list.append((kw[0], 1 - kw[1]))

    dbc.connect()
    for kw in new_list:
        dbc.updateTable(kw[0], kw[1])
    dbc.disconnect()


def giveFeedback(username, sessionID, user_role, text, individual_text):  # Text of the session
    # dictionary = constructDictionary()
    dictionary = tm.insertIntoDict()
    # for word in dictionary:
    #     print(word, dictionary[word])

    prediction = findProbabilities(dictionary, preProcessing(text))
    keyword_list = tm.extractKeywords(individual_text)
	

    result = ""

    if (prediction == 0 and case_role == "plaintiff") or (prediction == 1 and case_role == "defendant"):
        result = "Success"
    elif (prediction == 1 and case_role == "plaintiff") or (prediction == 0 and case_role == "defendant"):
        result = "Fail"
		
    most_influential_keywords = [duo[0] for duo in keyword_list]
    # print("The keywords that were the most influential in the case script:")
    # print(most_influential_keywords)
    dbc.connect()
    res_list = dbc.findEffectiveKeywordsFromDB(most_influential_keywords)
    #  res_list[0] : negative, res_list[1] : positive

    neg_keywords = ""
    pos_keywords = ""
    for key in res_list[1]:
        pos_keywords += key + ", "
    pos_keywords = pos_keywords[:-1]
    pos_keywords = pos_keywords[:-1]

    for key in res_list[0]:
        neg_keywords += key + ", "
    neg_keywords = neg_keywords[:-1]
    neg_keywords = neg_keywords[:-1]


    dbc.addFeedbackToDB(username, sessionID, result, pos_keywords, neg_keywords, user_role)

    dbc.disconnect()


main()
