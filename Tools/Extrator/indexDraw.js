var fs = require('fs'),
    xml2js = require('xml2js');
const { createCanvas, loadImage } = require('canvas')
const _ = require('lodash');
// const CanvasGifEncoder = require('canvas-gif-encoder');

// import fs from 'fs';
// import xml2js from 'xml2js';
// import pkg from 'canvas';
// const { createCanvas, loadImage } = pkg;

// import _ from 'lodash';
// import CanvasGifEncoder from 'canvas-gif-encoder';

const inputFolder = "viewer";
const outFolder = "viewer_output";

const w = 5200;
const h = 8000;
const canvas = createCanvas(w, h);
const ctx = canvas.getContext('2d');
ctx.imageSmoothingEnabled = false;

const showHitboxes = false;
const gif = false;

readFiles(inputFolder + '/', function(filename, content) {
    if (filename.includes('.xml'))
        parseXml(filename)
}, function(err) {
    throw err;
});

function mapFile(fileData, fileName, image) {

    const gap = 15;
    const fontSize = 12;
    ctx.font = fontSize + 'px Impact';

    fileData.xml.bitmap = fileData.xml.bitmap.map(current => current.$);
    if (fileData.xml.hitbox)
        fileData.xml.hitbox = fileData.xml.hitbox.map(current => current.$);

    const sprites = _.groupBy(fileData.xml.bitmap, 'spriteName');
    const hitboxs = _.groupBy(fileData.xml.hitbox, 'spriteName');

    let currentX = gap;
    let currentY = gap;

    let canvasWidth = gap;
    let canvasHeight = gap;
    Object
        .keys(sprites)
        .forEach(name => {
            const labels = _.groupBy(sprites[name], 'label');
            Object
                .keys(labels)
                .forEach(labelName => {
                    if (labelName.includes('Start')) {
                        const endLabel = labelName.replace('Start', 'End');
                        // const frames = _.sortBy(labels[labelName], 'index');
                        // var lastIndex = +_.last(frames).index;

                        // const endFrames = _.sortBy(labels[labelName], 'index');
                        // endFrames.forEach(f => {
                        //     f.index = ++lastIndex;
                        // })
                        // labels[labelName].concat(endFrames);
                        labels[labelName].concat(endLabel);
                        delete labels[endLabel];

                        // labels[labelName] = _.sortBy(labels[labelName], 'index');
                    }
                })

            Object
                .keys(labels)
                .forEach(labelName => {
                    currentY += gap;

                    ctx.fillStyle = '#554433';
                    ctx.fillText(
                            _.snakeCase(
                                labelName ?
                                labelName.replace("_Sprite", "").replace("Start", "").replace("End", "") :
                                name.replace("_Sprite", "").replace("Start", "").replace("End", "")).replace(/_/g, ' ').toUpperCase(), gap, currentY
                        )
                        // ctx.fillText(name + "_" + labelName + "|End", gap, currentY)
                    index = 0;

                    currentX = gap;

                    let maxHeight = 0;
                    let maxWidth = 0;
                    labels[labelName].forEach(sprite => {
                        sprite.index = +sprite.index;
                        sprite.ssWidth = +sprite.ssWidth;
                        sprite.ssHeight = +sprite.ssHeight;
                        sprite.ssX = +sprite.ssX;
                        sprite.ssY = +sprite.ssY;
                        sprite.anchorX = +sprite.anchorX;
                        sprite.anchorY = +sprite.anchorY;

                        if (sprite.ssHeight > maxHeight)
                            maxHeight = sprite.ssHeight;

                        if (sprite.ssWidth > maxWidth)
                            maxWidth = sprite.ssWidth;
                    })

                    const frames = _.sortBy(labels[labelName], 'index');

                    // if (gif)
                    //     if (labelName.toLowerCase().includes("idle") || labelName.toLowerCase().includes("stand"))
                    //         createGif(fileName.replace('.xml', '') + "_" + labelName, image, frames, 148, 125)

                    frames.forEach(sprite => {

                        // let xPos = (maxWidth / 2) - (sprite.ssWidth / 2) - sprite.anchorX;
                        // let yPos = (maxHeight / 2) - (sprite.ssHeight / 2) - sprite.anchorY;

                        let xPos = 0;
                        let yPos = maxHeight - sprite.ssHeight;

                        ctx.drawImage(image, sprite.ssX, sprite.ssY, sprite.ssWidth, sprite.ssHeight, currentX + xPos, currentY + gap + yPos, sprite.ssWidth, sprite.ssHeight)
                        if (showHitboxes && showHitboxeshitboxs[name]) {
                            ctx.fillStyle = '#27F727';
                            const hbs = hitboxs[name].filter(hitbox => hitbox.index == sprite.index)
                            if (hbs) {
                                hbs.forEach(e => {
                                    if (+e.type == 1) {
                                        ctx.strokeStyle = '#27F727';
                                    }
                                    if (+e.type == 2) {
                                        ctx.strokeStyle = '#c94747';
                                    }
                                    if (+e.type == 3) {
                                        ctx.strokeStyle = '#12c9f0';
                                    }
                                    ctx.beginPath();
                                    ctx.rect((currentX + +sprite.anchorX) + (+e.x), currentY + +gap + yPos + (+e.y) + +sprite.anchorY, +e.width, +e.height)
                                    ctx.stroke();
                                })
                            }
                        }
                        currentX += sprite.ssWidth + gap;

                        if (currentX > canvasWidth)
                            canvasWidth = currentX;
                    })
                    currentY += maxHeight + fontSize + gap;


                    if (currentY > maxHeight)
                        canvasHeight = currentY;
                })

        })
    return [canvasWidth, canvasHeight]
}

function parseXml(filename) {
    var parser = new xml2js.Parser();
    fs.readFile(`${inputFolder}/${filename}`, function(err, data) {
        parser.parseString(data, (err, result) => {
            loadImage(`${inputFolder}/${filename.replace('xml', 'png')}`).then((image) => {

                if(!result.xml) {
                    return;
                }

                ctx.clearRect(0, 0, w, h);

                // ctx.fillRect(0, 0, w, h);
                ctx.fillStyle = '#554433';
                ctx.fillText("Copyright (C) 2020, Cellar Door Games Inc. Full Metal Furies is a trademark of Cellar Door Games Inc. All Right Reserved. Ripped by Ralidon", 15, 15)

                const [canvasWidth, canvasHeight] = mapFile(result, filename, image);

                const fileCanvas = createCanvas(canvasWidth, canvasHeight)
                const fileCtx = fileCanvas.getContext('2d')

                fileCtx.drawImage(
                    canvas, 0, 0
                );

                const out = fs.createWriteStream(`${outFolder}/${filename.replace('.xml', '')}.png`)
                const stream = fileCanvas.createPNGStream()
                stream.pipe(out)
                out.on('finish', () => {
                    console.log('The PNG file was created.')
                })

                out.on('error', () => {
                    console.log('ERROR PNG file was NOT created.')
                })

                console.log('Done');
            })
        });
    });
}

// function createGif(filename, image, frames, maxWidth, maxHeight) {
//     const gifCanvas = createCanvas(maxWidth, maxHeight);
//     const gifCtx = gifCanvas.getContext('2d');
//     gifCtx.imageSmoothingEnabled = false;

//     const options = {
//         alphaThreshold: 0.1,
//         quality: 1,
//     };

//     // const encoder = new CanvasGifEncoder(maxWidth, maxHeight, options);

//     let stream = fs.createWriteStream(`${outFolder}/${filename}.gif`);
//     encoder.createReadStream().pipe(stream);

//     encoder.begin();

//     let index = 0;
//     frames.forEach(sprite => {
//         // if (index == frames.length - 2)
//         // return;
//         // gifCtx.clearRect(0, 0, maxWidth, maxHeight);
//         gifCtx.fillStyle = "red";
//         gifCtx.fillRect(0, 0, maxWidth, maxHeight);

//         let xPos = (maxWidth / 2) - (sprite.ssWidth / 2) - sprite.anchorX;
//         let yPos = (maxHeight / 2) - (sprite.ssHeight / 2) - sprite.anchorY;

//         gifCtx.drawImage(image, sprite.ssX, sprite.ssY, sprite.ssWidth, sprite.ssHeight, xPos, yPos, sprite.ssWidth, sprite.ssHeight)

//         encoder.addFrame(gifCtx, 100);
//         index++;
//     })

//     encoder.end();
// }

function readFiles(dirname, onFileContent, onError) {
    fs.readdir(dirname, function(err, filenames) {
        if (err) {
            onError(err);
            return;
        }
        filenames.forEach(function(filename) {
            fs.readFile(dirname + filename, 'utf-8', function(err, content) {
                if (err) {
                    onError(err);
                    return;
                }
                onFileContent(filename, content);
            });
        });
    });
}