# -*- coding: utf-8 -*

class VoiceListParser:

    def parse(self, path: str) -> None:
        array = []
        with open(path, 'r') as file:
            with open("../CustomPlayer/VoiceList.cs", 'w') as parsedFile:
                voiceName = ""

                for line in file.readlines():
                    for word in line:
                        if word == ' ':
                            break;
                        else: 
                            voiceName += word

                    duplicate = False;
                    for arrayElement in array:
                        if arrayElement == voiceName: 
                            duplicate = True;
                            break;

                    if not duplicate: array.append(voiceName)

                    voiceName = ""
                    
                tab = 4 * " "
                parsedFile.write("//This file was parsed by SpeechParser\n")
                parsedFile.write("namespace CustomPlayer\n" + "{\n" + tab + "public enum VoiceList\n" + tab + "{\n")
                for line in array:
                    parsedFile.write(2 * tab + line + ",\n")
                parsedFile.write(tab + "};\n" + "}")



parser = VoiceListParser()
parser.parse("Speeches.txt")