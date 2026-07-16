The SHSM project is currently in the Alpha phase. The following lists the potential risks identifiable at this stage and the mitigation measures already implemented or planned.

# Technical Risks
| Risk | Description | Mitigation |
|---|---|---|
| Cross-Language Client Compatibility | HTTP API calls for client languages ​​(Java, Go, Node.js, Python) may behave inconsistently across different versions or environments. | Minimal usable examples have been written for each language in the Beta phase to verify the basic call chain. |
| Unmanaged Memory Stability | The project relies on libsodium pointer operations, which may cause server-side crashes in certain environments (documented in the README). | Some functions that may trigger crashes have been restricted in client applications; further investigation will be conducted based on actual feedback. |
| Arweave Dependency Availability | The Arweave network may be unavailable due to network congestion or service interruptions, affecting anchoring operations. | There's no exact replacement for Arweave/Walrus/Filecoin for now. In terms of cost and data immutability, Arweave kinda wins among all 3. If blockchain was not to be used, depending on the region where this is hosted, people may need to afford more expensive VPS hosting and replace the Arweave dependency to that of DBMS oriented. This is because more expensive VPS often comes with better cybersecurity guarantees. |
| Insufficient Test Coverage | Current testing mainly covers core functional paths; boundary conditions and abnormal scenarios have not yet been systematically tested. | In the Beta phase, test cases are gradually added based on actual user feedback, rather than covering all boundaries in advance in the Alpha phase. |

# Project Risks
| Risk | Description | Mitigation |
|---|---|---|
| Limited Maintainer Time | The project is currently maintained by individuals in their spare time, which may limit progress. | Control the project scope, prioritize core functions, and postpone non-essential functions to later phases. |
| Insufficient Community Participation | The current phase lacks external contributors, and the project relies on the individual efforts of maintainers. | Develop contribution guidelines in the Beta phase to lower the barrier to participation through documentation. |
| Uncertain Funding Outcome | An application for NLnet funding has been submitted, but the outcome is unknown. | Regardless of the funding outcome, the project will continue at the current pace; if funding is received, Beta phase development will be accelerated. |

# Strategies Adopted
To address the above risks, the SHSM project currently adopts the following strategies:

1. **Scope Control**: Limit the scope of functionality in the Alpha phase to core available functions, and postpone non-essential functions.
2. **Iterative Development**: Do not require all issues to be resolved in the Alpha phase; instead, leave boundary cases and expansion requirements to be handled in the Beta phase based on actual feedback.
3. **Documentation first**: Reduce user confusion and the risk of misuse through clear definitions and documentation.
