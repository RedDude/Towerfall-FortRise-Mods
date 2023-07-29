var fs = require('fs'),
  sharp = require('sharp'),
  xml2js = require('xml2js');

var _ = require('lodash');

const execSync = require('child_process').execSync;

var inputFolder = 'input'
var outputFolder = 'output'

parseArcherXml(inputFolder + '/atlas.xml', atlas => {
    if (atlas)
      saveAtlas(atlas, outputFolder)
});

function saveAtlas(atlas, outputPlace) {
  atlas.TextureAtlas.SubTexture.forEach(a => {
    crop(a, outputPlace + '/', inputFolder + '/' + atlas.TextureAtlas.$.imagePath)
  })
}

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
  outputImage = out + outputImage;
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
    parser.parseString(data.toString(), function (err, result) {
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
