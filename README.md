# Configurações necessárias:

Para que seja possivel utilizar a minimal API para salvar os arquivos no seu blobstorage é necessário logar na sua conta azure, criar um storage account e visualizar o recurso,
no menu temos um grupo chamado <b> Security + Networking </b> nele podemos visualizar um item chamado Access Keys conforme demonstrado na imagem

Após acessar o item Access Keys o Azure nos fornece 2 chaves com as Strings de Conexão  já prontas para usar, é só clicar em show e ele já permite que seja copiado

Com a string de conexão é só colar ela no arquivo appsettings.json no valor da chave blobstorage
