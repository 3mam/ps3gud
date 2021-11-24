import { readFileSync, createWriteStream, mkdirSync, existsSync } from 'fs'
import { stdout, exit } from 'process'
import fetch from 'node-fetch'
import Progress from 'node-fetch-progress'
import { ArgumentParser } from 'argparse'

let loadJson = file => JSON.parse(readFileSync(file))
let toUpper = str => str.toUpperCase()
let mkdir = name => { try { mkdirSync(name, { recursive: true }) } catch (_) { } }
let getFileFromUrl = url => url.split('/').slice(-1)[0]

let argument = () => {
  let parser = new ArgumentParser({
    description: 'PlayStation 3 Games Update Downloader'
  })
  let { version } = loadJson('./package.json')
  parser.add_argument('-v', { action: 'version', version })
  parser.add_argument('-i', '--id', { nargs: '*', help: 'games id' })
  parser.add_argument('-d', '--download', { help: 'download folder' })
  return parser.parse_args()
}

let exitIfNotExist = downloadFolder => {
  if (!existsSync(downloadFolder)) {
    console.error(`Folder ${downloadFolder} for download don\'t exist!`)
    exit(-1)
  }
}

let getItemList = (gamesList, idList) =>
  gamesList.filter(
    ({ id }) => idList.includes(id)
  )

let checkIsIdOnList = (itemList, idList) => {
  let idItemList = itemList.map(({ id }) => id)
  idList.forEach(id => {
    if (!idItemList.includes(id)) {
      console.error(`${id} don't exist!`)
      exit(-1)
    }
  })
}

let download = async (url, folder, name, cursorLine) => {
  let fileName = getFileFromUrl(url)
  let response = await fetch(url)
  let progress = new Progress(response, { throttle: 100 })
  progress.on('progress', (p) => {
    stdout.clearScreenDown()
    stdout.cursorTo(0, cursorLine)
    stdout.write(`${name} sie:${p.totalh}/${p.doneh} speed:${p.rateh} ${Math.round(p.progress * 100)}%`)
  })
  response.body.pipe(createWriteStream(`${folder}/${fileName}`))
}

/** MAIN */
let args = argument()
exitIfNotExist(args.download)

let gamesList = loadJson('./db.json')
let idLit = args.id.map(v => toUpper(v))
let itemList = getItemList(gamesList, idLit)
checkIsIdOnList(itemList, idLit)
itemList.forEach(({ id, name, version, url }, i) => {
  let folders = `${args.download}/${id} ${name}/${version}`
  mkdir(folders)
  download(url, folders, `${name} path:${version}`, i)
})