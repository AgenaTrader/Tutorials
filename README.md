![AgenaTrader](./sources/images/logo_100.png)

# Live System
To get started, check out: [https://agenatrader.github.io/Tutorials/](https://agenatrader.github.io/Tutorials/)

# Documentation
MkDocs is a fast, simple and downright gorgeous static site generator that's geared towards building project documentation.
Documentation source files are written in [Markdown](https://guides.github.com/features/mastering-markdown/), and configured with a single YAML configuration file.
The tutorials in the sources directory of this repository, are built with mkdocs and publicly hosted on GitHub Pages at [https://agenatrader.github.io/Tutorials/](https://agenatrader.github.io/Tutorials/).

## Getting Started
### Editor
To edit these files you can use the GitHub Online Editor but we recommend to use a separate text editor.
We recommend the text editor **Atom**. [You can download it here.](https://atom.io)

![Atom](./sources/images/logo_atom.png)

### Installation
The following things are prerequisites:
* Install [GitHub Client](https://desktop.github.com) and create a GitHub user
* Install [Python](https://www.python.org/downloads/)
* Install Python package [mkdocs](http://www.mkdocs.org) using pip

### Parsing markdown to static website
Clone the master repository to your local computer.

Tutorials are written in markdown format and stored in the sources directory of this repository.
To parse these markdown files into static websites we need mkdocs to do this work for us. mkdocs will parse these markdown files, translate it into static websites and store it into the site directory.
All configuration is done in a single [YAML configuration file](mkdocs.yml) to configure the parsing process.
Open Console, navigate to your YAML configuration file and build the markdown files using the mkdocs build command.
```bash
mkdocs build
```
This will create a new directory named *site*.
If you are getting a warning that the *site directory* has already created, you can use the `clean` parameter to delete this directory before recreation.
```bash
mkdocs build --clean
```

### Bugfixing links for images
If you are using *<img> tags* in your markdown file, mkdocs is creating wrong links for these images, so we need to change this by **replacing ./media/ to ../media/** manually.
Another option is to do this automatic by using **sed** on linux or macintosh:
```bash
sed -i .bak -e 's%./media/%../media/%g' site/Example_Indicator_Condition_Strategy_Basic/README/index.html  && rm site/Example_Indicator_Condition_Strategy_Basic/README/index.html.bak
sed -i .bak -e 's%./media/%../media/%g' site/Example_Indicator_Condition_Strategy_Advanced/README/index.html  && rm site/Example_Indicator_Condition_Strategy_Advanced/README/index.html.bak
```

## Deploy
### Create branch gh-pages
Our static website will be displayed via the gh-pages branch. So we need to create the branch by using the following commands via command line.
```bash
git checkout master # you can avoid this line if you are in master
git subtree split --prefix output -b gh-pages # create a local gh-pages branch containing the splitted output folder
git push -f origin gh-pages:gh-pages # force the push of the gh-pages branch to the remote gh-pages branch at origin
git branch -D gh-pages # delete the local gh-pages
```

### Deploy the website
We have created a branch called *gh-pages* and all data from this branch will be displayed on GitHub Pages.
Commit now your changes into the master repository and sync your master repository.
Now execute the shell script to build the webiste and deploy it to the remote gh-branch.
```bash
./buildanddeploy.sh
```

Congrats! We are done. All changes are now online on GitHub Pages.
