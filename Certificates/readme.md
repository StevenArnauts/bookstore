-- links:
interesting: https://www.phildev.net/ssl/managing_ca.html
this finally worked: https://gist.github.com/fntlnz/cf14feb5a46b2eda428e000157447309
Holy potatoes!!!!! .NET's hostname validator only matches wildcards within a dNSName label of the Subject Alternative Names extension (see https://github.com/dotnet/corefx/issues/34061)

(works on osx and windows)

-- create ca root key
openssl genrsa -des3 -out ca.key 4096

-- create ca root certificate
openssl req -x509 -new -nodes -key ca.key -sha256 -days 1024 -out ca.crt -config ca.cnf

-- create key for request
openssl genrsa -out local.key 2048

-- create certificate request
openssl req -new -key local.key -out local.csr -config local.cnf -extensions req_ext

-- issue certificate
openssl x509 -req -in local.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out local.crt -days 500 -sha256 -extfile local.cnf -extensions req_ext

-- create pfx
openssl pkcs12 -export -out local.pfx -inkey local.key -in local.crt

