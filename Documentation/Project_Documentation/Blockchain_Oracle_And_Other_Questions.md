# Blockchain Oracle
By default, the SHSM server acts as a **customized Arweave oracle**, handling the anchoring of cryptographic commitments to the Arweave network.

To support additional blockchains or decentralized storage networks, a separate, offline-capable oracle implementation would need to be added. The current design does not include native support for multiple blockchains.

# Other Questions

## 1. Using PEM/Base64 Cryptographic Key Materials
SHSM does not support CA-encoded cryptographic key materials (e.g., PEM) by default.

To use key materials with SHSM, they must be converted to:
- Non-CA encrypted Base64 format (Preferred)
- Raw binary data format

This is because the current implementation expects plain binary or Base64‑encoded key data, rather than structured certificate formats like PEM.
