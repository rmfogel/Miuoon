

export class CloseSearch
{//חיפוש דומה
public static LevenshteinDistance(s: string, t: string):number {
    let n: number = s.length;
    let m: number = t.length;
    let d=new Array(n+1)
    for(let i=0;i<n+1;i++)
    d[i]=new Array(m+1)
    if (n == 0) {
        return m;
    }
    if (m == 0) {
        return n;
    }
    for (let i = 0; i <= n; d[i][0] = i++)
        ;
    for (let j = 0; j <= m; d[0][j] = j++)
        ;
    for (let i = 1; i <= n; i++) {
        for (let j = 1; j <= m; j++) {
            let cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
            d[i][j] = Math.min(
                Math.min(d[i - 1][j] + 1, d[i][j - 1] + 1),
                d[i - 1][j - 1] + cost);
        }
    }
    return d[n][m];
}
}