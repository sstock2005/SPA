
# SPA

<p align="center">
  <img width="460" height="300" src="https://raw.githubusercontent.com/sstock2005/SPA/main/emoji.png">
</p>

The **S**ecure **P**assword **A**pplication is an end-to-end encrypted server and client password manager that I wanted to see if I could make. It still has a long way to go but is functional for the time being.



## Features

- No local storage of data
    - No passwords are stored locally, they are all on the server.
- All communcations are heavily encrypted
    - Every network communcation between the client and the network is encrypted with ~~AES~~ XOR technology (Working on AES).
- Full user support system with custom encryption key for each user's passwords. The server admin won't even know what the passwords are in the database.

## Encryption

1. The user logs in or registers. This sends a request to the server using HTTP/S and XOR algorithm with a `mainkey` variable that the client and server share. This variable changes every hour due to UNIX time.
    - If the user is registering, the server will decode the request with the `mainkey` variable, and then attempt to insert the username, password, and master key. Because a key of a XOR algorithm should be fairly long, the server stores the mainkey x3 encoded in Base64 instead of plaintext to make it longer. The server will then respond with an encrypted response using the `mainkey` variable.
        - If the user already exists, the server will return a `USER_ALREADY_EXIST` error.
        - If the user is registered successfully, the server will return a `REGISTER_SUCCESS`.
    - If the user is logging in, the client will send a request encrypted with the `mainkey` variable. The server will check if the user exists, if the user does then the server will check the password against the hashed version of the password in the database. All responses from the server then on are encrypted twice. All important variables are encrypted with the user's encryption key (or master key) and then encrypted with the `mainkey` before being sent back.
        - If the user is not found, the server will return `USER_NOT_FOUND`.
        - If the user is found, but the password hash doesn't match, the server will return `INCORRECT_PASS`.
        - If the user is found, and the server hashes match, the server will respond with `LOGIN_SUCCESS`.

2. Once the user is authenticated, the client will then request all passwords for that user. It only sends one argument in the request. Which is the `username`. A bad-actor could easily send a request but it would not be readable to them because the response it double encrypted with the user's key and the `mainkey` variable.

3. Once a user clicks on a saved password to view more info and to copy the password, then and ONLY then will the client finally decrypt the password into plaintext.

## Examples
<p align="center">
  <img src="https://raw.githubusercontent.com/sstock2005/SPA/main/encryption.png">
Communication between client and server is encrypted.
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/sstock2005/SPA/main/database_example.png">
<br>Database Example. No plaintext passwords here!
</p>

## Deployment

To deploy this project you will need to setup MySQL and install [this database](https://github.com/sstock2005/SPA/raw/main/server/database.sql).
You will also need to change the host variable in [Security.cs](https://github.com/sstock2005/SPA/blob/main/client/Security.cs) if you don't want to just host it locally or on  a different port.  
`pip install -r requirements.txt`  
Then just start the server and you should be good to go!  

## Security Vulnerabilities

1. Langauge
    - C# has always been my favorite langauge but it is very easily hacked. The way I wrote this, an attacker can modify the client, but they cannot get access to anything unless the user input's their Master Key on a modified client.

2. Encryption (or lack thereof)
    - I was forced to use XOR's algorithm for the current version of SPA because of the difficulties for setting up cross-langauge AES Encryption. So the encryption is fairly week even with doubling it. I plan to change this ASAP.

## Roadmap
```
- Change XOR to AES256
- Recode Client GUI (It does not look the best)
- Add a delete function (duh)
```
