import pymorphy2

def normalize(string):
    RuLemmatizer = pymorphy2.MorphAnalyzer()
    return RuLemmatizer.normal_forms(string)[0].upper()