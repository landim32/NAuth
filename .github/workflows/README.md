# GitHub Actions Workflows

Este diretório contém os workflows do GitHub Actions para CI/CD do NAuth.

## ?? Workflows Disponíveis

### docker-build.yml
**Propósito**: Build e push automático de imagens Docker

**Triggers**:
- Push para branches `main` e `develop`
- Pull requests para `main`
- Tags no formato `v*` (ex: v1.0.0)

**O que faz**:
1. ? Build da imagem Docker da API (NAuth.API)
2. ? Build da imagem Docker do PostgreSQL
3. ? Push para GitHub Container Registry (ghcr.io)
4. ? Tagging automático baseado em:
   - Branch name
   - PR number
   - Semantic version (tags)
   - Git SHA

**Imagens geradas**:
- `ghcr.io/<owner>/nauth/nauth-api`
- `ghcr.io/<owner>/nauth/nauth-postgres`

**Tags aplicadas**:
- `main` - Última build da branch main
- `develop` - Última build da branch develop
- `pr-123` - Build de Pull Request
- `v1.0.0` - Tag de versão específica
- `1.0` - Major.Minor da versão
- `1` - Major da versão
- `main-abc1234` - Branch + SHA do commit

## ?? Permissões Necessárias

O workflow usa `GITHUB_TOKEN` que é fornecido automaticamente pelo GitHub com as seguintes permissões:
- `contents: read` - Ler o código do repositório
- `packages: write` - Escrever no GitHub Container Registry

## ?? Container Registry

As imagens são publicadas no GitHub Container Registry (ghcr.io).

### Como usar as imagens

#### Pull da imagem
```bash
docker pull ghcr.io/<owner>/nauth/nauth-api:main
docker pull ghcr.io/<owner>/nauth/nauth-postgres:main
```

#### Usar no docker-compose
```yaml
services:
  nauth-api:
    image: ghcr.io/<owner>/nauth/nauth-api:v1.0.0
    # ... outras configurações
    
  postgres:
    image: ghcr.io/<owner>/nauth/nauth-postgres:v1.0.0
    # ... outras configurações
```

## ?? Como Funciona

### Build em Pull Request
```
PR aberta ? Workflow executa ? Build das imagens ? Sem push
```
Útil para validar que as imagens constroem corretamente.

### Build em Push para Main/Develop
```
Push ? Workflow executa ? Build das imagens ? Push para registry
```
Imagens são publicadas com tags da branch.

### Build de Release (Tag)
```
Tag v1.0.0 ? Workflow executa ? Build ? Push com múltiplas tags
```
Cria tags: `v1.0.0`, `1.0`, `1`, `latest`

## ??? Build Cache

O workflow usa GitHub Actions cache para acelerar builds:
- Cache de layers do Docker
- Reutilização entre builds
- Restauração automática

## ?? Monitoramento

### Ver status dos workflows
1. Acesse: `https://github.com/<owner>/<repo>/actions`
2. Selecione o workflow "Docker Build and Push"
3. Veja os runs e logs

### Badges
Adicione ao README.md:
```markdown
[![Docker Build](https://github.com/<owner>/<repo>/actions/workflows/docker-build.yml/badge.svg)](https://github.com/<owner>/<repo>/actions/workflows/docker-build.yml)
```

## ?? Customização

### Adicionar novos triggers
Edite a seção `on:` em `docker-build.yml`:
```yaml
on:
  push:
    branches: [ main, develop, staging ]
  schedule:
    - cron: '0 0 * * 0'  # Todo domingo à meia-noite
```

### Adicionar testes antes do build
```yaml
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Run tests
        run: dotnet test
        
  build-and-push:
    needs: test
    runs-on: ubuntu-latest
    # ... resto do workflow
```

### Multi-arquitetura (ARM64)
```yaml
- name: Set up QEMU
  uses: docker/setup-qemu-action@v3
  
- name: Build and push
  uses: docker/build-push-action@v5
  with:
    platforms: linux/amd64,linux/arm64
    # ... outras configurações
```

## ?? Troubleshooting

### Build falha
1. Verifique os logs no GitHub Actions
2. Teste localmente: `docker-compose build`
3. Verifique o Dockerfile

### Push falha
1. Verifique permissões do GITHUB_TOKEN
2. Verifique se o container registry está habilitado
3. Verifique Settings > Actions > General > Workflow permissions

### Cache não funciona
1. Limpe o cache: Settings > Actions > Caches
2. Verifique se `cache-from` e `cache-to` estão configurados

## ?? Recursos Adicionais

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Build Push Action](https://github.com/docker/build-push-action)
- [GitHub Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry)

## ?? Próximos Passos

Possíveis melhorias futuras:
- [ ] Adicionar testes automatizados
- [ ] Adicionar scan de segurança (Trivy/Snyk)
- [ ] Deploy automático para staging
- [ ] Notificações no Slack/Discord
- [ ] Performance benchmarks
- [ ] Validação de schemas

## ?? Dicas

1. **Use tags semânticas**: `v1.0.0` ao invés de `1.0.0`
2. **Teste localmente**: Antes de push, teste com `act` (GitHub Actions locally)
3. **Cache é seu amigo**: Use cache para builds mais rápidos
4. **Mantenha imagens pequenas**: Otimize Dockerfiles
5. **Documente mudanças**: Atualize CHANGELOG ao criar tags

## ?? Contribuindo

Para adicionar ou modificar workflows:

1. Teste localmente se possível
2. Documente as mudanças
3. Adicione comentários no workflow YAML
4. Atualize este README
5. Crie um Pull Request
