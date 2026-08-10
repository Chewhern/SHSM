# AES current vulnerabilities and SM4 potential vulnerabilities
In **libsodium** documentation, **AES** algorithm can be considered to be insecure or not recommended to be used if there's no proper advanced computing chips.

**SM4** on the other hand is a block cipher algorithm developed by China. It's currently unknown whether **SM4** had the same vulnerabilities like **AES**.

# What if there're no proper hardware support?
The fallback symmetric encryption algorithms need to be stream ciphers like XChaCha20 or XSalsa20 and paired them with MAC (Message Authentication Code)
algorithms. This applies only to international use cases.

If it's China, they have to have their own version of stream ciphers. 
