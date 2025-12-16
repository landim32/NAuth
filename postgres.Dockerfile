FROM postgres:16-alpine

# Copiar o script de inicialização para o diretório de init do PostgreSQL
COPY nauth.sql /docker-entrypoint-initdb.d/

# O PostgreSQL executará automaticamente todos os scripts SQL em /docker-entrypoint-initdb.d/
# na primeira inicialização do container
