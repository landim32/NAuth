FROM postgres:16-alpine

# Copiar o script SQL para o diretório de inicialização do PostgreSQL
# Este diretório executa scripts apenas na primeira inicialização
COPY nauth.sql /docker-entrypoint-initdb.d/nauth.sql
