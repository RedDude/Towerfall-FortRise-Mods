var fs = require('fs'),
  sharp = require('sharp'),
  jsonxml = require('jsontoxml'),
  xml2js = require('xml2js');

var _ = require('lodash');

const execSync = require('child_process').execSync;

const XMLExtract = require('xml-extract');
var inputFolder = 'input'
var filename = 'menuatlas.xml'
var outputFolder = 'output'

var currentMenuSpriteData = {};
var currentSpriteData = {};
var spriteDataParts = {};
var spriteDataMenuParts = {};
var globalArcherData = {};
var globalArcherDataParsed = {};
var archerSpriteData = {};
var archerSpriteMenuData = {};
var gemsSprites = {};
// readFiles(inputFolder+'/', function(filename, content) {
// if(filename.includes('.xml'))
// parseXml(filename)
// }, function(err) {
// throw err;
// });
var type = 'Archer'
var packerConfigTemplate = fs.readFileSync('packer-config.toml').toString();

parseArcherXml(inputFolder + '/archerData.xml', archerData => {
  parseArcherXml(inputFolder + '/atlas.xml', atlas => {
    parseArcherXml(inputFolder + '/menuatlas.xml', menuAtlas => {
      parseArcherXml(inputFolder + '/spriteData.xml', spriteData => {
        parseArcherXml(inputFolder + '/menuSpriteData.xml', menuSpriteData => {
          globalArcherDataParsed = archerData;

          archerData.Archers[type].forEach(currentArcherData => {

            if(!currentArcherData.Name0 && !currentArcherData.Corpse)
              return;

            var archerName =  !currentArcherData.Name0 ? currentArcherData.Corpse[0] 
              : _.startCase(_.capitalize(currentArcherData.Name0[0] + " " + currentArcherData.Name1[0]));
            var outputPlace = outputFolder + '/' + archerName;
            if (archerName == "Quote")
              return;

            if (!fs.existsSync(outputPlace)) {
              fs.mkdirSync(outputPlace);
            }
            if (!fs.existsSync(outputPlace+"/images")) {
              fs.mkdirSync(outputPlace+"/images");
            }

            archerSpriteData[archerName] = [];
            // archerSpriteMenuData[archerName] = [];

            currentArcherData.Aimer?.forEach(w => {
              saveFromAtlas(w, atlas, outputPlace);
            })

            currentArcherData.Sprites?.forEach(w => {
              if(w.Body)
                saveFromSpriteData(w.Body[0], spriteData, atlas, outputPlace, archerName)
              saveFromSpriteData(w.HeadNormal[0], spriteData, atlas, outputPlace, archerName)
              saveFromSpriteData(w.HeadNoHat[0], spriteData, atlas, outputPlace, archerName)
              saveFromSpriteData(w.HeadCrown[0], spriteData, atlas, outputPlace, archerName)
              if(w.Bow)
                saveFromSpriteData(w.Bow[0], spriteData, atlas, outputPlace, archerName)
            })

            currentArcherData.Hat?.forEach(w => {
              saveFromAtlas(w.Normal[0], atlas, outputPlace)
              saveFromAtlas(w.Blue[0], atlas, outputPlace)
              saveFromAtlas(w.Red[0], atlas, outputPlace)
            })

            currentArcherData.Portraits?.forEach(w => {
              if(w.NotJoined)
                saveFromAtlas(w.NotJoined[0], menuAtlas, outputPlace)
              saveFromAtlas(w.Joined[0], menuAtlas, outputPlace)
              saveFromAtlas(w.Win[0], menuAtlas, outputPlace)
              saveFromAtlas(w.Lose[0], menuAtlas, outputPlace)
            })

            currentArcherData.Statue?.forEach(w => {
              saveFromAtlas(w.Image[0], atlas, outputPlace)
              saveFromAtlas(w.Glow[0], atlas, outputPlace)
            })


            currentArcherData.Gems?.forEach(w => {
              if (w.Menu[0] == w.Gameplay[0]) {
                gemsSprites[archerName] = w.Menu[0]
              }
              saveFromSpriteData(w.Gameplay[0], spriteData, atlas, outputPlace, archerName)
              saveFromSpriteData(w.Menu[0], menuSpriteData, menuAtlas, outputPlace, archerName)
            })

            if(currentArcherData.Corpse)
              saveFromSpriteData(currentArcherData.Corpse, spriteData, atlas, outputPlace, archerName);

          });

          fs.readFile(inputFolder + '/archerData.xml', (err, data) => {
            XMLExtract(data.toString(), type, true, (err, d) => {
              var parser = new xml2js.Parser();
              parser.parseString(d, (err, result) => {
                if(!result[type].Name0 && !result[type].Corpse)
                  return;
                var archerName = !result[type].Name0 ? result[type].Corpse[0] 
                : _.startCase(_.capitalize(result[type].Name0[0] + " " + result[type].Name1[0]));
                if (gemsSprites[archerName]) {
                  d = d.replace('</Menu>', '_MENU</Menu>');
                }
                var outputPlace = outputFolder + '/' + archerName;
                fs.writeFile(outputPlace + '/' + "archerData.xml", d, err => {
                })
              })
            })
          })

          var gems = Object.values(gemsSprites);
          fs.readFile(inputFolder + '/spriteData.xml', (err, data) => {
            XMLExtract(data.toString(), 'sprite_string', true, (err, d) => {
              var parser = new xml2js.Parser();
              parser.parseString(d, (err, result) => {
                var id = result?.sprite_string?.$.id;
                if (gems.includes(id)) {
                  id += '_MENU';
                  d = d.replace(result?.sprite_string?.$.id, id);
                }
                spriteDataParts[id] = d;
              })
            })

            XMLExtract(data.toString(), 'sprite_int', true, (err, d) => {
              var parser = new xml2js.Parser();
              parser.parseString(d, (err, result) => {
                spriteDataParts[result?.sprite_int?.$.id] = d;
              })
            })

            Object.keys(currentSpriteData).forEach(key => {
              archerSpriteData[currentSpriteData[key]].push(
                spriteDataParts[key]
              )
            })

            Object.keys(archerSpriteData).forEach(archerName => {
              let asp = archerSpriteData[archerName].join('\n\n\t');
              let finalSpriteData =
                "<SpriteData>\n" + asp + "\n</SpriteData>";

              let outputPlace = outputFolder + '/' + archerName;
              fs.writeFileSync(outputPlace + '/' + "spriteData.xml", finalSpriteData)
              if(fs.existsSync(outputPlace + '/atlas.png')){
                fs.unlinkSync(outputPlace + '/atlas.png')
              }
              
              var packerConfig = packerConfigTemplate.toString();
              packerConfig = packerConfig
                                .replace(/output_path = \"([\/]?)output\/[\w\s]+\"/g, `output_path = "output\/${archerName}"`)
                                .replace(new RegExp(/  \"([\/]?)output\/[\w\s\/]+\"/g, 'g'), `   "output\/${archerName}\/images"`);

              // fs.writeFileSync('packer-config.toml', packerConfig)
              // code = execSync('cluttered.exe config --input packer-config.toml')
              // fs.readFile(outputPlace + '/atlas.xml', (err, atlasDataFile) => {
              //   var atlasData = atlasDataFile.toString();
              //   atlasData = atlasData.replace(/-/g, '\/')
              //               .replace(new RegExp(outputPlace+"/images/", 'g'), '');
              //   fs.writeFileSync(outputPlace + '/atlas.xml', atlasData)
              //   fs.unlinkSync(outputPlace + '/atlas.json')
              // });

            });
          })

        });
      });
    })
  })
})

function saveFromAtlas(name, atlas, outputPlace) {
  atlas.TextureAtlas.SubTexture.forEach(a => {
    if (a.$.name == name) {
      crop(a, outputPlace + '/', inputFolder + '/' + atlas.TextureAtlas.$.imagePath)
      return;
    }
  })
}

function saveFromSpriteData(id, spriteData, atlas, outputPlace, archerName) {
  spriteData?.SpriteData?.sprite_string?.forEach(a => {
    if (a?.$?.id == id) {
      if (a.Texture)
        saveFromAtlas(a.Texture, atlas, outputPlace);
      if (a.RedTexture)
        saveFromAtlas(a.RedTexture, atlas, outputPlace);
      if (a.BlueTexture)
        saveFromAtlas(a.BlueTexture, atlas, outputPlace);
      if (a.RedBody)
        saveFromAtlas(a.RedBody, atlas, outputPlace);
      if (a.BlueBody)
        saveFromAtlas(a.BlueBody, atlas, outputPlace);
      if (a.RedCorpse)
        saveFromAtlas(a.RedCorpse, atlas, outputPlace);
      if (a.BlueCorpse)
        saveFromAtlas(a.BlueCorpse, atlas, outputPlace);
      if (a.RedTeam)
        saveFromAtlas(a.RedTeam, atlas, outputPlace);
      if (a.BlueTeam)
        saveFromAtlas(a.BlueTeam, atlas, outputPlace);
      if (a.Flash)
        saveFromAtlas(a.Flash, atlas, outputPlace);
      if (currentSpriteData[a?.$?.id]) {
        currentSpriteData[a?.$?.id + "_MENU"] = archerName;
        return;
      }
      currentSpriteData[a?.$?.id] = archerName;
      return;
    }
  })
  spriteData?.SpriteData?.sprite_int?.forEach(a => {
    if (a?.$?.id == id) {
      saveFromAtlas(a.Texture, atlas, outputPlace);
      if (a.RedTexture)
        saveFromAtlas(a.RedTexture, atlas, outputPlace);
      if (a.BlueTexture)
        saveFromAtlas(a.BlueTexture, atlas, outputPlace);
      if (a.RedBody)
        saveFromAtlas(a.RedBody, atlas, outputPlace);
      if (a.BlueBody)
        saveFromAtlas(a.BlueBody, atlas, outputPlace);
      if (a.RedCorpse)
        saveFromAtlas(a.RedCorpse, atlas, outputPlace);
      if (a.BlueCorpse)
        saveFromAtlas(a.BlueCorpse, atlas, outputPlace);
      if (a.Flash)
        saveFromAtlas(a.Flash, atlas, outputPlace);
      currentSpriteData[a?.$?.id] = archerName;
      return;
    }
  })
}


function crop(xmlLine, out, source) {
  const line = xmlLine.$;
  let outputImage = line.name + '.png';
  outputImage = outputImage.replace(new RegExp('/', 'g'), '-');
  outputImage = out +"images/"+ outputImage;
  // file name for cropped image
  let originalImage = source;

  sharp(originalImage).extract({
    width: parseInt(line.width),
    height: parseInt(line.height),
    left: parseInt(line.x),
    top: parseInt(line.y)
  })
    .toFile(outputImage)
    .then(function (new_file_info) {
      console.log("Image cropped and saved");
    })
    .catch(function (err) {
      console.log("An error occured: " + err.message);
    });
}

function parseArcherXml(xmlFilename, callback, store) {
  var parser = new xml2js.Parser();
  fs.readFile(`${xmlFilename}`, function (err, data) {
    if (store)
      store = data
    parser.parseString(data, function (err, result) {
      console.log(err)
      callback(result, xmlFilename)
    });
  });
}

function readFiles(dirname, onFileContent, onError) {
  fs.readdir(dirname, function (err, filenames) {
    if (err) {
      onError(err);
      return;
    }
    filenames.forEach(function (filename) {
      fs.readFile(dirname + filename, 'utf-8', function (err, content) {
        if (err) {
          onError(err);
          return;
        }
        onFileContent(filename, content);
      });
    });
  });
}
