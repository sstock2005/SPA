from flask import Flask, request, g
import mysql.connector, bcrypt, base64
from encryption import encrypt, decrypt
import time, json
app = Flask(__name__)

db_config = {
    "host": "localhost",
    "user": "root",
    "password": "",
    "database": "data",
}

def get_db():
    if not hasattr(g, 'db'):
        g.db = mysql.connector.connect(**db_config)
        g.cursor = g.db.cursor(buffered=True)
    return g.cursor, g.db

@app.before_request
def before_request():
    request.cursor, request.db = get_db()

@app.teardown_request
def teardown_request(exception):
    cursor = getattr(request, 'cursor', None)
    db = getattr(request, 'db', None)
    if cursor is not None:
        cursor.close()
    if db is not None:
        db.close()

def getUnix():
    unix_timestamp = int(time.time())
    unix_hour = (unix_timestamp // 3600) % 24
    return str(unix_hour).zfill(2)

main_key = "845a443a87bbefae39693724eb82c0d6-{}".format(getUnix())

def getuserkey(user):
    request.cursor.execute("SELECT enckey FROM spa WHERE username = %s", (user,))
    result = request.cursor.fetchone()
    if result is not None:
        return result[0]
    
def hashpass(password):
    salt = bcrypt.gensalt()
    hashed_password = bcrypt.hashpw(password.encode('utf-8'), salt)
    return hashed_password.decode('utf-8')

def checkpass(password, hashed):
    return bcrypt.checkpw(password.encode('utf-8'), hashed.encode('utf-8'))

def b64(input):
    encoded_bytes = base64.b64encode(input.encode('utf-8'))
    return encoded_bytes.decode('utf-8')

@app.route('/')
def home():
    return "hello world", 200

@app.route('/login', methods=['POST'])
def login():
    try:
        data = json.loads(decrypt(request.data.decode(), main_key))
        username = data.get("username")
        entered_password = data.get("password")
        request.cursor.execute("SELECT password FROM spa WHERE username = %s", (username,))
        result = request.cursor.fetchone()

        if result is not None:
            stored_hashed_password = result[0]
            if checkpass(entered_password, stored_hashed_password):
                user_encrypted = encrypt("LOGIN_SUCCESS", getuserkey(username))
                return encrypt(user_encrypted, main_key), 200
            else:
                return encrypt("INCORRECT_PASS", main_key), 401
        else:
            return encrypt("USERNAME_NOT_FOUND", main_key), 404
    except mysql.connector.Error as err:
        print(err)
        return encrypt(f"Error: {err}", main_key), 500

@app.route('/register', methods=['POST'])
def register():
    try:
        data = json.loads(decrypt(request.data.decode(), main_key))
        username = data.get("username")
        entered_password = data.get("password")
        masterkey = data.get("key")
        enckey = b64(b64(b64(masterkey)))
        password = hashpass(entered_password)
        request.cursor.execute("SELECT COUNT(*) FROM spa WHERE username = %s", (username,))
        count = request.cursor.fetchone()[0]

        if count > 0:
            return encrypt("USER_ALREADY_EXIST", main_key), 409
        insert_query = "INSERT INTO spa (username, password, enckey) VALUES (%s, %s, %s)"
        values = (username, password, enckey)
        request.cursor.execute(insert_query, values)
        request.db.commit()
        return encrypt("REGISTER_SUCCESS", main_key), 200
    except mysql.connector.Error as err:
        return encrypt(f"Error: {err}", main_key), 500

@app.route('/api/get', methods=['POST'])
def apiget():
    try:
        results = []
        data = json.loads(decrypt(request.data.decode(), main_key))
        username = data.get("username")
        insert_query = "SELECT * FROM passwords WHERE owner = %s"
        values = (username,)
        request.cursor.execute(insert_query, values)
        columns = [desc[0] for desc in request.cursor.description]

        for row in request.cursor.fetchall():
            if row != None:
                result_dict = {}
                for i in range(len(columns)):
                    result_dict[columns[i]] = row[i]
                results.append(result_dict)
        if results != None and results != []:
            result_json = json.dumps(results)
            return encrypt(result_json, main_key), 200
        else:
            return encrypt("NO_PASSWORDS", main_key), 200
    except mysql.connector.Error as err:
        return encrypt(f"Error: {err}", main_key), 500

@app.route('/api/add', methods=['POST'])
def apiadd():
    try:
        data = json.loads(decrypt(request.data.decode(), main_key))
        username = data.get("username")
        user_key = getuserkey(username)
        url = decrypt(data.get("url"), user_key)
        useremail = decrypt(data.get("useremail"), user_key)
        password = decrypt(data.get("password"), user_key)
        request.cursor.execute("INSERT INTO passwords (owner, url, useremail, password) VALUES (%s, %s, %s, %s)", (username, url, useremail, encrypt(password, user_key)))
        request.db.commit()
        return encrypt(encrypt("PASSWORD_ADD_SUCCESS", user_key), main_key), 200
    
    except mysql.connector.Error as err:
        return encrypt(f"Error: {err}", main_key), 500

if __name__ == '__main__':
    app.run('0.0.0.0', 4444, False)
