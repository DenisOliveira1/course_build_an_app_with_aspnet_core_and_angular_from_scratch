
export class Tools{
    public static titleCaseText(text : string) : string{
        if (text.length > 2){
            return  text.charAt(0).toUpperCase() + text.substr(1).toLowerCase();
        }
        return text;
    }
}
