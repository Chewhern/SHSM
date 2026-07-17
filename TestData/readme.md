# English
The data within the **Users** folder had been pre-generated with the following code snippets and using **SPKI's TL or ML AU's App**.

```
RevampedKeyPair MyED25519KeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPrivateKey.txt", MyED25519KeyPair.PrivateKey);
//File.WriteAllBytes(AppContext.BaseDirectory + "/SubDSAPrivateKey.txt", MyED25519KeyPair.PrivateKey); <- Linux
File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPublicKey.txt", MyED25519KeyPair.PublicKey);
//File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPublicKey.txt", MyED25519KeyPair.PublicKey); <- Linux
MyED25519KeyPair.Clear();
```
Or
```
ED448RevampedKeyPair MyKeyPair = SecureED448.GenerateED448RevampedKeyPair();
File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPrivateKey.txt", MyKeyPair.PrivateKey);
File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPublicKey.txt", MyKeyPair.PublicKey);
//File.WriteAllBytes(AppContext.BaseDirectory + "/SubDSAPrivateKey.txt", MyKeyPair.PrivateKey); <- Linux
//File.WriteAllBytes(AppContext.BaseDirectory + "/SubDSAPublicKey.txt", MyKeyPair.PublicKey); <- Linux
MyKeyPair.Clear();
```

This application needs to pre-anchor two types of JSON Data String to Arweave.

[AU's Info](https://bpji76zxzyurba6rjxnglhfbbyiyqvf7s6ef73vj3gakbmuj6lxa.arweave.net/C9KP-zfOKRCD0U3aZZyhDhGIVL-XiF_uqdmAoLKJ8u4)

[Signed Sub Public Key's Info](https://sfh3ctrep4nlctrdnpjkesnp6r6f2xft3bbvhpklt5irxktippia.arweave.net/kU-xTiR_GrFOI2vSokmv9HxdXLPYQ1O9S59RG6poe9A)

One can do it either with Arweave's NodeJS SDK or can use C#'s SDK for the purpose. However in either case, you need to have actual AR or Arweave's cryptocurrency in order to anchor the data to Arweave.

This set of test data will only be valid latest until the end of November (For Signed Sub Public Key's Info) and valid latest until the end of December (For AU's Info).

- AU Info's Arweave ID: C9KP-zfOKRCD0U3aZZyhDhGIVL-XiF_uqdmAoLKJ8u4
- Signed Sub Public Key's Info's Arweave ID: kU-xTiR_GrFOI2vSokmv9HxdXLPYQ1O9S59RG6poe9A

# 中文
**Users** 文件夹中的数据已使用以下代码片段和 **SPKI 的 TL 或 ML AU 的应用程序** 预先生成。

```
RevampedKeyPair MyED25519KeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPrivateKey.txt", MyED25519KeyPair.PrivateKey);
//File.WriteAllBytes(AppContext.BaseDirectory + "/SubDSAPrivateKey.txt", MyED25519KeyPair.PrivateKey); <- Linux
File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPublicKey.txt", MyED25519KeyPair.PublicKey);
//File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPublicKey.txt", MyED25519KeyPair.PublicKey); <- Linux
MyED25519KeyPair.Clear();
```
或者
```
ED448RevampedKeyPair MyKeyPair = SecureED448.GenerateED448RevampedKeyPair();
File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPrivateKey.txt", MyKeyPair.PrivateKey);
File.WriteAllBytes(AppContext.BaseDirectory + "\\SubDSAPublicKey.txt", MyKeyPair.PublicKey);
//File.WriteAllBytes(AppContext.BaseDirectory + "/SubDSAPrivateKey.txt", MyKeyPair.PrivateKey); <- Linux
//File.WriteAllBytes(AppContext.BaseDirectory + "/SubDSAPublicKey.txt", MyKeyPair.PublicKey); <- Linux
MyKeyPair.Clear();
```

此应用程序需要预先将两种类型的 JSON 数据字符串锚定到 Arweave 中。

[AU's Info](https://bpji76zxzyurba6rjxnglhfbbyiyqvf7s6ef73vj3gakbmuj6lxa.arweave.net/C9KP-zfOKRCD0U3aZZyhDhGIVL-XiF_uqdmAoLKJ8u4)

[Signed Sub Public Key's Info](https://sfh3ctrep4nlctrdnpjkesnp6r6f2xft3bbvhpklt5irxktippia.arweave.net/kU-xTiR_GrFOI2vSokmv9HxdXLPYQ1O9S59RG6poe9A)

您可以使用 Arweave 的 NodeJS SDK 或 C# SDK 来实现此目的。但无论使用哪种方式，您都需要拥有实际的 AR 或 Arweave 加密货币，才能将数据锚定到 Arweave。

这组测试数据的有效期最晚至 11 月底（针对已签名子公钥的信息）和 12 月底（针对 AU 的信息）。

- AU Info's Arweave ID: C9KP-zfOKRCD0U3aZZyhDhGIVL-XiF_uqdmAoLKJ8u4
- Signed Sub Public Key's Info's Arweave ID: kU-xTiR_GrFOI2vSokmv9HxdXLPYQ1O9S59RG6poe9A
