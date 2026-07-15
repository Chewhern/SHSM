using ASodium;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SHSM_ServerApp.SHSMDataModel;

namespace SHSM_ServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerInformation : ControllerBase
    {
        [HttpGet]
        public SupportedAlgorithmsModel GetSupportedAlgorithmsDetails() 
        {
            //The integer indices will be used to determine which algorithms will be used in this SHSM.
            //If "AbleToSupportSecureAES" ended up in false
            //must avoid using AES algorithms at all cost due to the side-channel attacks presence without proper hardware support
            SupportedAlgorithmsModel MyModel = new SupportedAlgorithmsModel();
            String ListOfAESAlgorithmsRawString = "AES256GCM,AEGIS256,AEGIS128L";
            String ListOfAEADAlgorithmsRawString = "XChaCha20Poly1305IETF,ChaCha20Poly1305IETF,ChaCha20Poly1305";
            String ListOfStreamCipherAlgorithmsRawString = "XChaCha20,XSalsa20,ChaCha20,ChaCha20IETF,Salsa20,Salsa12,Salsa8";
            String ListOfMACAlgorithmsRawString = "HMACSHA512256,HMACSHA512,HMACSHA256,Poly1305";
            String ListOfSupportedDigitalSignatureAlgorithmsRawString = "ED25519(libsodium),ED448(BouncyCastle),RSA(.NET)";
            String ListOfKeyExchangeAlgorithmsRawString = "SealedBox-XSalsa20Poly1305,SealedBox-XChaCha20Poly1305";
            MyModel.AbleToSupportSecureAES = SodiumSecretAeadAES256GCM.IsAES256GCMAvailable();
            MyModel.ListOfAESAlgorithms = ListOfAESAlgorithmsRawString.Split(",");
            MyModel.ListOfAESAlgorithmsIndices = new int[] { 0, 1, 2 };
            MyModel.ListOfAEADAlgorithms = ListOfAEADAlgorithmsRawString.Split(",");
            MyModel.ListOfAEADAlgorithmsIndices = new int[] { 0, 1, 2 };
            MyModel.StreamCipherAlgorithms = ListOfStreamCipherAlgorithmsRawString.Split(",");
            MyModel.StreamCipherAlgorithmsIndices = new int[] { 0, 1, 2, 3, 4, 5, 6 };
            MyModel.MACAlgorithms = ListOfMACAlgorithmsRawString.Split(",");
            MyModel.MACAlgorithmsIndices = new int[] { 0, 1, 2, 3 };
            MyModel.DigitalSignatureAlgorithms = ListOfSupportedDigitalSignatureAlgorithmsRawString.Split(",");
            MyModel.DigitalSignatureAlgorithmsIndices = new int[] { 0, 1, 2};
            MyModel.KeyExchangeAlgorithms = ListOfKeyExchangeAlgorithmsRawString.Split(",");
            MyModel.KeyExchangeAlgorithmsIndices = new int[] { 0, 1};
            MyModel.KEMAlgorithm = "Hybrid KEM - XWing";
            return MyModel;
        }

        /*
        ===Sample JSON Data===
        Most likely the AbleToSupportSecureAES will be true if it's running on modern version of virtual private server..
        {
	        "AbleToSupportSecureAES":false,
	        "ListOfAESAlgorithms":
	        ["AES256GCM","AEGIS128L","AEGIS256"],
	        "ListOfAESAlgorithmsIndices":[0,1,2],
	        "ListOfAEADAlgorithms":["ChaCha20Poly1305","ChaCha20Poly1305IETF","XChaCha20Poly1305IETF"],
	        "ListOfAEADAlgorithmsIndices":[0,1,2],
	        "StreamCipherAlgorithms":["XChaCha20"," XSalsa20"," ChaCha20"," ChaCha20IETF"," Salsa20"," Salsa12"," Salsa8"],
	        "StreamCipherAlgorithmsIndices":[0,1,2,3,4,5,6],
	        "MACAlgorithms":
	        ["HMACSHA512256","HMACSHA512","HMACSHA256","Poly1305"],
	        "MACAlgorithmsIndices":[0,1,2,3],
	        "DigitalSignatureAlgorithms":["ED25519(libsodium)","ED448(BouncyCastle)","RSA(.NET)"],
	        "DigitalSignatureAlgorithmsIndices":[0,1,2],
	        "KeyExchangeAlgorithms":
	        ["X25519(libsodium)","X448(BouncyCastle)","SealedBox-XSalsa20Poly1305","SealedBox-XChaCha20Poly1305"],
	        "KeyExchangeAlgorithmsIndices":
	        [0,1,2,3],
	        "KEMAlgorithm":"Hybrid KEM - XWing"
        }
         */
    }
}
