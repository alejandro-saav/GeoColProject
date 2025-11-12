export function toPascalCase(str: string): string {
    if (!str) return str;
    if (str == "bogotá, d.c.") return "Bogotá, D.C"
    return str.split(" ").map(word => word[0].toUpperCase() + word.slice(1)).join(" ");
    // return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
}