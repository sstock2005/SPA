import base64

def encrypt(input, key):
    output = []
    
    for i in range(len(input)):
        xor_num = ord(input[i]) ^ ord(key[i % len(key)])
        output.append(chr(xor_num))
    
    encrypted_text = ''.join(output)
    encrypted_bytes = encrypted_text.encode('utf-8')
    encrypted_base64 = base64.b64encode(encrypted_bytes).decode('utf-8')
    
    return encrypted_base64

def decrypt(encrypted_base64, key):
    encrypted_bytes = base64.b64decode(encrypted_base64.encode('utf-8'))
    encrypted_text = encrypted_bytes.decode('utf-8')

    output = []
    
    for i in range(len(encrypted_text)):
        xor_num = ord(encrypted_text[i]) ^ ord(key[i % len(key)])
        output.append(chr(xor_num))
    
    decrypted_text = ''.join(output)
    
    return decrypted_text