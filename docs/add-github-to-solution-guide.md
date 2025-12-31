# Como Adicionar a Pasta .github à Solução Visual Studio

## Método 1: Usando o Script PowerShell (Recomendado)

Execute o script PowerShell que foi criado:

```powershell
.\add-github-to-solution.ps1
```

Ou se sua solução tiver um nome diferente:

```powershell
.\add-github-to-solution.ps1 -SolutionPath "CaminhoParaSuaSolucao.sln"
```

## Método 2: Manualmente no Visual Studio

### Passo a Passo:

1. **Abra a solução NAuth.sln no Visual Studio**

2. **Adicione uma Nova Solution Folder:**
   - Clique com o botão direito na solução (no topo do Solution Explorer)
   - Selecione `Add` ? `New Solution Folder`
   - Nomeie como `.github`

3. **Adicione o Sub-folder workflows:**
   - Clique com o botão direito na pasta `.github`
   - Selecione `Add` ? `New Solution Folder`
   - Nomeie como `workflows`

4. **Adicione os arquivos existentes:**
   - Clique com o botão direito na pasta `workflows`
   - Selecione `Add` ? `Existing Item...`
   - Navegue até `.github\workflows\`
   - Selecione os arquivos:
     - `publish-nuget.yml`
     - `README.md`
   - Clique em `Add`

5. **Adicione arquivos na raiz do .github (se houver):**
   - Clique com o botão direito na pasta `.github`
   - Selecione `Add` ? `Existing Item...`
   - Adicione quaisquer arquivos na raiz de `.github`

6. **Salve a solução:**
   - Pressione `Ctrl+Shift+S` ou
   - `File` ? `Save All`

## Método 3: Editar o arquivo .sln manualmente

Se preferir editar diretamente o arquivo de solução:

1. **Feche o Visual Studio**

2. **Abra NAuth.sln em um editor de texto**

3. **Adicione estas linhas após a última definição de projeto:**

```
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = ".github", ".github", "{GUID-AQUI}"
	ProjectSection(SolutionItems) = preProject
		.github\workflows\publish-nuget.yml = .github\workflows\publish-nuget.yml
		.github\workflows\README.md = .github\workflows\README.md
	EndProjectSection
EndProject
```

**Importante:** Substitua `{GUID-AQUI}` por um GUID único. Você pode gerar um em: https://www.guidgenerator.com/

Exemplo de GUID: `{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}`

4. **Salve o arquivo**

5. **Reabra a solução no Visual Studio**

## Verificação

Após adicionar, você deve ver a seguinte estrutura no Solution Explorer:

```
Solution 'NAuth'
??? NAuth.ACL
??? NAuth.API
??? NAuth.Application
??? NAuth.Domain
??? NAuth.DTO
??? NAuth.Infra
??? NAuth.Infra.Interfaces
??? NAuth.Test
??? .github
    ??? workflows
        ??? publish-nuget.yml
        ??? README.md
```

## Vantagens de Adicionar à Solução

? **Visibilidade:** Arquivos ficam visíveis no Solution Explorer
? **Edição Fácil:** Pode editar os arquivos diretamente no Visual Studio
? **Versionamento:** Facilita o controle de versão
? **Organização:** Mantém todos os arquivos do projeto organizados
? **Busca:** Os arquivos aparecem na busca do Visual Studio

## Arquivos que Devem Estar na Pasta .github

Certifique-se de que os seguintes arquivos existem:

- `.github/workflows/publish-nuget.yml` - Workflow de publicação
- `.github/workflows/README.md` - Documentação do workflow

## Troubleshooting

### Os arquivos não aparecem após adicionar?
- Verifique se os caminhos dos arquivos estão corretos
- Recarregue a solução: `File` ? `Close Solution` ? `File` ? `Open` ? `Project/Solution`

### Erro "File not found"?
- Certifique-se de que os arquivos realmente existem no filesystem
- Verifique se os caminhos são relativos à raiz da solução

### Pasta .github não é reconhecida pelo Git?
- A pasta `.github` é especial e deve ser reconhecida automaticamente pelo Git
- Verifique se não está no `.gitignore`

## Git Status

Após adicionar, verifique o status do Git:

```bash
git status
```

Você deve ver:
```
modified:   NAuth.sln
```

## Commit das Mudanças

```bash
git add NAuth.sln
git commit -m "chore: add .github folder to solution"
git push origin main
```

## Notas Importantes

?? **Backup:** Sempre faça backup do arquivo `.sln` antes de editar manualmente
?? **GUID Único:** Cada Solution Folder precisa de um GUID único
?? **Case Sensitive:** O Git é case-sensitive, então `.github` deve ser minúsculo
?? **Paths:** Use barras invertidas (`\`) nos caminhos do Windows no arquivo .sln

## Recursos Adicionais

- [Visual Studio Solution Files](https://docs.microsoft.com/en-us/visualstudio/extensibility/internals/solution-dot-sln-file)
- [Solution Folders](https://docs.microsoft.com/en-us/visualstudio/ide/solutions-and-projects-in-visual-studio)
- [GitHub Actions](https://docs.github.com/en/actions)
