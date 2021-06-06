import psycopg2

conn = None


def connect():
    """ Connect to the PostgreSQL database server """
    global conn
    if conn is None:
        try:
            # connect to the PostgreSQL server
            print('Connecting to the PostgreSQL database...')
            conn = psycopg2.connect(database="lawvr",
                                    user="lawvr",
                                    password="lawvr123",
                                    host="lawvrdb.cou5tvcgh993.us-east-2.rds.amazonaws.com",
                                    port="5432")

        except (Exception, psycopg2.DatabaseError) as error:
            print(error)


def disconnect():
    global conn
    if conn is not None:
        conn.close()
        print('Database connection closed.')
        conn = None


def createTable():
    cursor = conn.cursor()
    sql = '''CREATE TABLE KEYWORD(
                       ID SERIAL PRIMARY KEY,
                       KEYWORD TEXT NOT NULL,
                       POINT FLOAT NOT NULL
                    )'''
    cursor.execute(sql)
    print("Table created successfully........")

    cursor.close()
    conn.commit()


def createDictTable():
    cursor = conn.cursor()
    sql = '''CREATE TABLE DICTIONARY(
                       ID SERIAL PRIMARY KEY,
                       KEYWORD TEXT NOT NULL,
                       POSITIVE INT NOT NULL,
                       NEGATIVE INT NOT NULL,
                       COUNT INT NOT NULL
                    )'''
    cursor.execute(sql)
    print("Table created successfully........")

    cursor.close()
    conn.commit()


def dropTable():
    cursor = conn.cursor()
    cursor.execute("DROP TABLE KEYWORD")
    print("Table dropped... ")
    conn.commit()


def fetchAll():
    cur = conn.cursor()
    cur.execute("SELECT * FROM KEYWORD ORDER BY ID")
    rows = cur.fetchall()
    print("The number: ", cur.rowcount)
    for row in rows:
        print(row)
    cur.close()


def fetchAllDict():
    cur = conn.cursor()
    cur.execute("SELECT * FROM DICTIONARY ORDER BY ID")
    rows = cur.fetchall()
    print("The number: ", cur.rowcount)
    # for row in rows:
    #     print(row)
    cur.close()
    return rows


def checkIfExist(keyword):
    # Check if the keyword is in the table
    cur = conn.cursor()
    cur.execute('SELECT COUNT(*) FROM KEYWORD WHERE KEYWORD = %s', (keyword,))
    result = cur.fetchall()
    cur.close()
    if result[0][0] == 0:
        return False
    else:
        return True


def insertToTable(keyword, point):
    # Insert keyword if not in table
    cur = conn.cursor()
    cur.execute("INSERT INTO KEYWORD (KEYWORD,POINT) VALUES (%s,%s )", (keyword, point))
    cur.close()
    conn.commit()


def insertToDictTable(dict):
    # Insert keyword if not in table
    cur = conn.cursor()
    for key in dict:
        cur.execute("INSERT INTO DICTIONARY (KEYWORD,POSITIVE,NEGATIVE,COUNT) VALUES (%s,%s,%s,%s)",
                    (key, dict[key][0], dict[key][1], dict[key][2]))
    cur.close()
    conn.commit()


def addValue(keyword, pointToAdd):
    # Update keyword value in table
    cur = conn.cursor()

    newPoint = pointToAdd

    cur.execute("SELECT POINT FROM KEYWORD WHERE (KEYWORD = %s)", (keyword,))
    row = cur.fetchone()
    if row:  # check whether the query returned a row
        newPoint += row[0]

    cur.execute("UPDATE KEYWORD SET POINT = %s WHERE KEYWORD = %s", (newPoint, keyword))
    cur.close()
    conn.commit()


def updateTable(keyword, pointToAdd):
    # If keyword is not in the table, adds it with an initial point. If it is in the table, adds the given value.
    if checkIfExist(keyword):
        addValue(keyword, pointToAdd)
    else:
        insertToTable(keyword, pointToAdd)


def findEffectiveKeywordsFromDB(keywords):  # sign is 0 or 1: 0 for negative points, 1 for positive.
    cur = conn.cursor()
    pos_list = []
    neg_list = []
    for keyword in keywords:
        cur.execute('SELECT POINT FROM KEYWORD WHERE KEYWORD = %s', (keyword,))
        res = cur.fetchone()
        if res is not None:
            if res[0] < 0:
                neg_list.append(keyword)
            if res[0] > 0:
                pos_list.append(keyword)
    cur.close()
    new_list = [neg_list, pos_list]
    return new_list


def getSessionScript(session_id):
    cur = conn.cursor()
    cur.execute('SELECT SPEECH FROM SPEECH_LOG WHERE SESSION_ID = %s', (session_id,))
    lines = cur.fetchall()
    text = ""
    for line in lines:
        text += line[0] + "\n"
    return text


def getSessionSpeakerScript(session_id, speaker_role):
    cur = conn.cursor()
    cur.execute('SELECT SPEECH FROM SPEECH_LOG WHERE SESSION_ID = %s AND SPEAKER_ROLE = %s', (session_id, speaker_role))
    lines = cur.fetchall()
    text = ""
    for line in lines:
        text += line[0] + "\n"
    return text

	
def addFeedbackToDB(username, sessionID, result, pos_keywords, neg_keywords, user_role):
    cur = conn.cursor()

    postgres_insert_query = """ INSERT INTO SESSION_FEEDBACKS (SESSION_ID, USER_NAME, RESULT, POSITIVE_KEYWORDS, NEGATIVE_KEYWORDS, USER_ROLE) VALUES (%s,%s,%s,%s,%s,%s)"""
    record_to_insert = (sessionID, username, result, pos_keywords, neg_keywords, user_role)
    cur.execute(postgres_insert_query, record_to_insert)
    cur.close()
    conn.commit()
	
	
	