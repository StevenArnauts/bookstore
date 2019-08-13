REM create ca key
openssl genrsa -des3 -out ca.key 4096

REM create ca root certificate
openssl req -x509 -new -nodes -key ca.key -sha256 -days 1024 -out ca.crt -config ca.cnf


