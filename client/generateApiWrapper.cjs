// generateApiWrapper.cjs
const fs = require('fs')
const path = require('path')

const apiRoot = path.join(__dirname, 'src/renderer/src/api')
const servicesDir = path.join(apiRoot, 'services')
const modelsDir = path.join(apiRoot, 'models')
const outputFile = path.join(apiRoot, 'api.ts')

if (!fs.existsSync(servicesDir)) {
  console.error("Services directory not found. Did you run 'npm run generate-api'?")
  process.exit(1)
}

// collect all service files
const serviceFiles = fs
  .readdirSync(servicesDir)
  .filter((f) => f.endsWith('.ts'))
  .map((f) => f.replace(/\.ts$/, ''))

// import everything from each service
const imports = serviceFiles
  .map((name) => `import * as ${name}Exports from "./services/${name}";`)
  .join('\n')

// merge into one api object
const apiObject = `
export const api = {
  ${serviceFiles.map((name) => `...${name}Exports`).join(',\n  ')}
};
`

// re-export all models
const modelExports = fs.existsSync(modelsDir)
  ? fs
      .readdirSync(modelsDir)
      .filter((f) => f.endsWith('.ts'))
      .map((f) => `export * from "./models/${f.replace(/\.ts$/, '')}";`)
      .join('\n')
  : ''

const fileContent = `${imports}\n\n${apiObject}\n\n${modelExports}\n`
fs.writeFileSync(outputFile, fileContent)
console.log('API wrapper generated successfully!')
