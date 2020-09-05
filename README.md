# Finder

[![Test](https://github.com/litefeel/Unity-Finder/workflows/Test/badge.svg)](https://github.com/litefeel/Unity-Finder/actions)
[![](https://img.shields.io/github/release/litefeel/Unity-Finder.svg?label=latest%20version)](https://github.com/litefeel/Unity-Finder/releases)
[![](https://img.shields.io/github/license/litefeel/Unity-Finder.svg)](https://github.com/litefeel/Unity-Finder/blob/master/LICENSE.md)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://paypal.me/litefeel)

Find Asset in Unity.


#### Shotscreen

![shotscreen](Documentation~/shotscreen1.png)


### Requirement

- Unity 2018.4+

### Install

- By NPM (Ease upgrade in Package Manager UI)**Recommend**

Find the manifest.json file in the Packages folder of your project and edit it to look like this:
``` js
{
  "scopedRegistries": [
    {
      "name": "My Registry",
      "url": "https://registry.npmjs.org",
      "scopes": [
        "com.litefeel"
      ]
    }
  ],
  "dependencies": {
    "com.litefeel.finder": "1.1.0",
    ...
  }
}
```

- By git url

Find the manifest.json file in the Packages folder of your project and edit it to look like this:
``` js
{
  "dependencies": {
    "com.litefeel.finder": "https://github.com/litefeel/Unity-Finder.git#1.1.0",
    ...
  }
}
```


### Support

* Create issues by issues page (https://github.com/litefeel/Unity-Finder/issues)
* Send email to me: litefeel@gmail.com
