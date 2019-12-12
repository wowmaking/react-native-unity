const fs = require('fs'),
  xml2js = require('xml2js');

const p = process.argv[2];

const parser = new xml2js.Parser();
const builder = new xml2js.Builder();

fs.readFile(p, function(err, data) {
  if (err) {
    throw err;
  }
  parser.parseString(data, function(err, result) {
    if (err) {
      throw err;
    }
    delete result.manifest.application[0].activity[0]['intent-filter'];
    fs.writeFile(p, builder.buildObject(result), function(err) {
      if (err) {
        throw err;
      }
    });
  });
});
