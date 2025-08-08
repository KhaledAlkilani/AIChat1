// generateApiWrapper.js (example)
const fs = require('fs')
const path = require('path')

const servicesDir = path.join(__dirname, 'src/api/services')
const outputFile = path.join(__dirname, 'src/api/api.ts')

// Ensure services directory exists
if (!fs.existsSync(servicesDir)) {
  console.error("Services directory not found! Run 'npm run generate-api' first.")
  process.exit(1)
}

// Step 1: Get all service files
const serviceFiles = fs
  .readdirSync(servicesDir)
  .filter((file) => file.endsWith('.ts'))
  .map((file) => file.replace('.ts', ''))

// Step 2: Import everything from each service
// e.g., "import * as PlayerServiceExports from './services/PlayerService';"
const imports = serviceFiles
  .map((name) => `import * as ${name}Exports from "./services/${name}";`)
  .join('\n')

// Step 3: Create an api object that merges all named exports from each file
// so you can do: api.PlayerServiceExports.getPlayerInfo()
const apiObject = `
export const api = {
  ${serviceFiles.map((name) => `...${name}Exports`).join(',\n  ')}
};
`

// Step 4: Export all models
const modelsDir = path.join(__dirname, 'src/api/models')
const modelFiles = fs.existsSync(modelsDir)
  ? fs
      .readdirSync(modelsDir)
      .filter((file) => file.endsWith('.ts'))
      .map((file) => `export * from "./models/${file.replace('.ts', '')}";`)
      .join('\n')
  : ''

const fileContent = `${imports}\n\n${apiObject}\n\n${modelFiles}\n`

fs.writeFileSync(outputFile, fileContent)
console.log('API wrapper generated successfully!')
