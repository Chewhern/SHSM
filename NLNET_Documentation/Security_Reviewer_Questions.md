# Purpose
This markdown file may help to answer some of the questions that security reviewers might ask.

# What attacks remain?
The following threats remain outside the current security scope of SHSM.

## Runtime Environment
Core and process dumps: This will expose the private and secret keys that linger within the SHSM server's memory.

## Deployment Assumptions
- Missing AppArmor
- Missing SELinux

Linux's ```seccomp, namespaces, AppArmor, SELinux``` or Windows' ```AppContainer, Windows Sandbox technologies, Restricted tokens / Job Objects (depending on the application)``` security sandboxing measures may be absent or not yet enable on them.

## Design tradeoffs
- Secrets lifetime: The default secret keys and private keys will linger in memory for 1 hour (Can be customized on the code). The client side may decide to extend the lifetime by another 1 hour or additional duration. **The default 1-hour lifetime can be shortened through code customization, and is not enforced by the SHSM server itself. This design choice is intentional—it allows flexibility—but also means that key lifetime management ultimately depends on the deploying environment.**
- Debugging detections: Linux and Windows have debugger presence detections. **However, it's not entirely sure whether enforcing this will give any actual security impact to SHSM server side.**

Debugging detection is not currently enforced, as its security benefit for the server-side implementation has not been established. This may be considered in future iterations..

## Out of scope
- Physical attacks
- Hardware compromise
- Kernel compromise
- Filesystem compromise

# Security improvements that can be measured
SHSM currently provides qualitative security improvements rather than standardized quantitative security metrics.

- fewer plaintext key copies? : If SHSM was used right, the SHSM client side will have fewer direct access to plaintext key.
- automatic zeroization? : If SHSM was used right and with right operation choice, the code will help to automatic zero the keys on the server side. If it's SHSM on the client side, other than using the client application, client may need to manually zero the keys on their end or if there's future application
- page protection? and memory isolation? : Libsodium's ```sodium_malloc, sodium_free, sodium_mprotect_noaccess, sodium_mprotect_readonly, sodium_mprotect_readwrite``` had been used to enforce the stated security measures.
- shorter key exposure windows? : If SHSM server code had been customized, the exposure windows can be shortened to less than 1 hour. If there's no customization, the exposure windows by default is 1 hour. 

```Resistance to accidental key leakage (e.g., during core dumps or process crashes) is not addressed in the current Alpha version. Addressing this would require additional engineering effort and is not part of the current project scope.```
