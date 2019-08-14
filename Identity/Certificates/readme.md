# Certificates

## Context
IdentityServer needs a secret to sign and validate tokens, we use certificates for that

Since we might deploy more than one instance, we must take care that the certificate is the same 
on all those instances.

Certificates eventually expire. Simply replacing a certificate that is about to expire is a problem, 
because all existing long lived tokens (refresh tokens) will be invalid because the signature 
can't be verified any more.

To solve this problem, IdentityServer supports "signing key rollover" 
(http://docs.identityserver.io/en/release/topics/crypto.html#signing-key-rollover): 
when a new certificate is installed, the old ones can still be used to validate earlier tokens.

### Environments
It's important that we have different certificates in every environment, especially production, 
otherwise tokens issued in development will be valid in production. This implies that the 
certificates cannot be part of the software itself (e.g. as embedded resources), because we 
need to vary them in every environment. 
They will have a separate life cycle.

### Passwords
The certificates that are generated are protected by a password. No need to say that this is an 
important secret.

## Implementation
DRAS supports using multiple certificates. 
At startup, all the .pfx files in the Certificates directory are loaded in ascending order
of their file names. All will be available for validating tokens, but only the last one in 
this list will be used to sign new tokens.

The directory containing the certificates can be configured in appsetttings.json:
```json
"IdentityServer": {
    "Certificates": {
      "Folder": ".\\Certificates",
      "Password": "doccle"
    } 
  }
```
The folder can be an absolute path, or one that is relative to the location of the main
assembly of the process.

## Procedure
When the signing certificate is about to expire, generate a new signing certificate 
using the command line in the Tools directory.
The script has an expiration date in it, so be sure to **change that to something meaningful**, like
one year in the future.
```
create_signing_certificate signing
```

This will generate a new file doccle-signing.pfx. Move that file to the Certificates folder, 
and add a sequence number suffix. Don't remove or overwrite the existing certificates, 
they are required to validate existing tokens.

If for some reason we would **want** to invalidate all tokens, we could remove all 
existing certificates.

# TO DO
Decide *how* certificates are deployed. I currently think that our ARM templates might be
the best option, to be discussed.
