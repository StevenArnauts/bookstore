REM create key for request
openssl genrsa -out local.key 2048

REM create certificate request
openssl req -new -key local.key -out local.csr -config local.cnf -extensions req_ext

REM issue certificate
openssl x509 -req -in local.csr -CA ..\ca\ca.crt -CAkey ..\ca\ca.key -CAcreateserial -out local.crt -days 500 -sha256 -extfile local.cnf -extensions req_ext

REM create pfx
openssl pkcs12 -export -out local.pfx -inkey local.key -in local.crt

