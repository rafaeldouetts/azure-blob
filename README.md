# Configurações necessárias:

Para que seja possivel utilizar a minimal API para salvar os arquivos no seu blobstorage é necessário logar na sua conta azure, criar um storage account e visualizar o recurso,
no menu temos um grupo chamado <b> Security + Networking </b> nele podemos visualizar um item chamado Access Keys conforme demonstrado na imagem

![image](https://github.com/JairJr/TechChallenge/assets/29376086/8ae4d87e-6799-4ef5-bc16-c5ced3f4b962)

Após acessar o item Access Keys o Azure nos fornece 2 chaves com as Strings de Conexão  já prontas para usar, é só clicar em show e ele já permite que seja copiado

![image](https://github.com/JairJr/TechChallenge/assets/29376086/0fc9972b-64ac-48d3-bd9b-85428ac17294)

Com a string de conexão é só colar ela no arquivo appsettings.json no valor da chave blobstorage

![image](https://github.com/JairJr/TechChallenge/assets/29376086/3b339842-3a17-4aa1-a8cc-4d5902bd897d)
