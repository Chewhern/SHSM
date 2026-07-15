# Operations that were feasible
There're about 8 operations that were feasible:
1. IP Config
2. Registration
3. ETLS
4. Public Key Cryptography
5. Secret Key Cryptography
6. Arweave
7. SHSM (Only used for removing registered SHSM user in advanced) 
8. API Key (Won't be mention here as this and password operations need to be customized)

To perform any operations, IP Config had to be done in advance.

**ETLS will be used for importing client side's encrypted private or secret keys.**

Any **Export** wording means the server side will use submitted signed export public key to encrypt in a way that only client can decrypt. It'll be primarily used for sending server's current session's private or secret keys to client side for future use. 

## Registration
1. Use SPKITL or SPKIML's AU App to get proper AU information.
2. Use AU information to anchor it with SPKITL or SPKIML's ON App that will help anchor to Arweave automatically.

If the first two can't be done, then you can use your own way to generate appropriate information.

3. Use the following code snippet or modify it accordingly to anchor the respective information to Arweave.
```
//Steps..
//1. Generate ED25519 or ED448 Key Pair for SubDSA..
//2. Sign the public key with AU's Signing private key via SPKITL or SPKIML AU App
//3. Upload and anchor the data to Arweave..
RevampedKeyPair MyED25519KeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();

File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPrivateKey.txt", MyED25519KeyPair.PrivateKey);
File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPublicKey.txt", MyED25519KeyPair.PublicKey);

MyED25519KeyPair.Clear();

//Kindly get this from Arweave's wallet private key or 's RSA private key.. as I can't expose my Arweave's private key here..
var rsaParams = new RSAParameters();
rsaParams.Modulus = new Byte[] {0x00,0x00};
rsaParams.Exponent = new Byte[] { 0x00,0x00 };
rsaParams.D = new Byte[] { 0x00,0x00};
rsaParams.P = new Byte[] { 0x00,0x00};
rsaParams.Q = new Byte[] { 0x00,0x00};
rsaParams.DP = new Byte[] { 0x00,0x00};
rsaParams.DQ = new Byte[] { 0x00,0x00};
rsaParams.InverseQ = new Byte[] { 0x00,0x00};

SubSignedPKModel MyModel = new SubSignedPKModel();
MyModel.SignedDigitalSignaturePublicKeyB64 = "LF9ksMKbYP6HXlBxdfxrpJmhB7sT/jPkwYkAuXkNCGJFEmZWZmPMXkXmCZJBhPN1C4I3QfYt/vEi8rmJ9/fPBpRShl9D/WZYQacfne6kqT3GWkz08Ouz31zyN/MDfDsw";
MyModel.DigitalSignatureAlgorithm = "ED25519";
MyModel.ValidDate_Year = 2026;
MyModel.ValidDate_Month = 11;
MyModel.ValidDate_Day = 30;

String JSONBodyString = JsonConvert.SerializeObject(MyModel);
(String Status,String TransactionID) MyStatus = await ArweaveSecureCreateAndPostDataHelper.UploadData(JSONBodyString, Base64URLEncodeDecodeHelper.Encode(rsaParams.Modulus), rsaParams.Modulus, rsaParams.Exponent, rsaParams.D, rsaParams.P, rsaParams.Q, rsaParams.DP, rsaParams.DQ, rsaParams.InverseQ);
```

4. Get **AU_Info's Arweave ID,AU Signed Sub DSA Public Key Arweave ID** and input it to the client application or refer to its underlying logic to mimic HTTP API call. This should add the information to memory.
5. Optionally, you can change the export public keys.

## ETLS to API Key Operations
These operations need to rely on users or developers to configure IP address and establish connection to the server.

Then registration will be the next key operation.

### Public Key Cryptography and Secret Key Cryptography Operations
**Public Key Cryptography and Secret Key Cryptography** once established or initialized, the SHSM on the server side will then start counting with a lease of 1 hour.

After 1 hour and there's no update on the lease, the server side will automatically remove the registered SHSM user and its associated information.

#### Arweave operation
You'll need to input RSA whole key into the SHSM server using **import** function.

Then only you can use the Arweave anchoring function.

This function was provided to ensure the private key stay contained and able to securely wiped out after use which determined by developers or users..
