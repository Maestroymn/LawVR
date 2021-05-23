import yake

from io import StringIO

# Extracting text from PDF
from pdfminer.converter import TextConverter
from pdfminer.layout import LAParams
from pdfminer.pdfdocument import PDFDocument
from pdfminer.pdfinterp import PDFResourceManager, PDFPageInterpreter
from pdfminer.pdfpage import PDFPage
from pdfminer.pdfparser import PDFParser

from fpdf import FPDF

from os import listdir
from os.path import isfile, join

import datetime
import random

import DBComm as dbc

path_to_cases = 'cases/'


def extractText(filename):  # From PDF files
    output_string = StringIO()
    with open(filename, 'rb') as in_file:
        parser = PDFParser(in_file)
        doc = PDFDocument(parser)
        rsrcmgr = PDFResourceManager()
        device = TextConverter(rsrcmgr, output_string, laparams=LAParams())
        interpreter = PDFPageInterpreter(rsrcmgr, device)
        for page in PDFPage.create_pages(doc):
            interpreter.process_page(page)

    return output_string.getvalue()


def extractKeywordsFromFile(filename):
    text = extractText(filename)
    return extractKeywords(text)


def extractKeywords(text):  # with Yake
    # Keyword extraction from text
    language = "tr"
    max_ngram_size = 1
    # deduplication_thresold = 0.9
    # deduplication_algo = 'seqm'
    # windowSize = 1
    numOfKeywords = 20

    custom_kw_extractor = yake.KeywordExtractor(lan=language,
                                                n=max_ngram_size,
                                                # dedupLim=deduplication_thresold,
                                                # dedupFunc=deduplication_algo,
                                                # windowsSize=windowSize,
                                                top=numOfKeywords,
                                                features=None)
    keywords = custom_kw_extractor.extract_keywords(text)

    return keywords


class Dataset:
    def __init__(self):
        self.data = []
        self.target = []

    def addToDataset(self, data, target):  # Takes a data(text) and target (0 oe 1) as input and
        # appends to the list of dataset
        self.data.append(data)
        self.target.append(target)


def createDataset():
    path_to_cases_0 = path_to_cases + '0/'
    path_to_cases_1 = path_to_cases + '1/'
    caseFiles0 = [f for f in listdir(path_to_cases_0) if isfile(join(path_to_cases_0, f))]
    caseFiles1 = [f for f in listdir(path_to_cases_1) if isfile(join(path_to_cases_1, f))]

    dataset = Dataset()  # Create dataset

    # Add to dataset
    # If the file is in the ../0/ directory, it's target will be 0 (result of the case)
    # If the file is in the ../1/ directory, it's target will be 1
    for case in caseFiles0:
        text = extractText(path_to_cases_0 + case)
        dataset.addToDataset(text, 0)

    for case in caseFiles1:
        text = extractText(path_to_cases_1 + case)
        dataset.addToDataset(text, 1)

    return dataset


def addToDataset(dataset, type, case):  # Type: 0 or 1, case: text of the case script
    caseID = datetime.datetime.now().strftime("%Y-%m-%d-%H-%M-%S-") + str(random.randint(0, 9))  # Just in case if
    # 2 people add a case at the same time

    dataset.addToDataset(case, type)

    # Save the case to a pdf file, add it to default directory
    pdf = FPDF()
    pdf.add_page()
    pdf.add_font('tr-arial', '', 'resources/tr-arial.ttf', uni=True)
    pdf.set_font('tr-arial', '', 11)

    text = case.replace(u"\t", u" ")
    text = text.replace(u"\x0c", u"")
    pdf.multi_cell(0, 5, text)

    # save the pdf with name .pdf
    pdf.output(path_to_cases + str(type) + "/" + caseID + ".pdf")


def insertIntoDict():
    dictionary = dict()
    dbc.connect()
    rows = dbc.fetchAllDict()
    for row in rows:
        dictionary[row[1]] = [row[2], row[3], row[4]]

    dbc.disconnect()
    # print(dictionary)

    return dictionary
