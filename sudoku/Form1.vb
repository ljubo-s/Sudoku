Imports System
Public Class Sudoku
    Public Class TB
        Public WithEvents t As New TextBox

        Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles t.KeyUp 't.TextChanged,

            If e.KeyCode = Keys.Back Or e.KeyCode = Keys.Delete Then
            Else
                If (Sudoku.dozvoljeni_znakovi.Contains(t.Text)) Then
                Else
                    MsgBox("Nedozvoljen simbol")
                    t.Text = ""
                End If
                If Val(t.Text) > 9 Then
                    MsgBox("Nedozvoljen simbol")
                    t.Text = ""
                Else
                End If
            End If
        End Sub
    End Class

    Public Class TBkandidat
        Public WithEvents t As New TextBox
        Public l As System.Windows.Forms.Label

        Sub New()
            t.BackColor = Color.PowderBlue
        End Sub
    End Class

    ' t(,) - matrica TextBoxova koja se prikazuje na formi
    Public t(,) As TB

    ' mat(,) - matrica u kojoj se radi, tj. trazi resenje i upisuju/brisu mogucnosti dok se ne dodje do resenja
    Public mat(,) As String

    ' z(,) - matrica u kojoj se cuva zadatak
    Public z(,) As String

    ' kandidati(,) - matrica u kojoj se cuvaju kandidati
    Public kandidat(,) As String
    Public k(,) As TBkandidat

    ' n,m velicina velike matrice 
    Public n, m As Integer

    ' n_malo, m_malo dimenzija malih matrica
    Public n_malo, m_malo As Integer

    Public res(,) As String

    ' dimenzija textboxova
    Public t_velicina As Integer
    Public t_font As Integer = 18
    Public preskok_n, preskok_m As Integer
    Public n_s, m_s As Integer
    Public s() As Char = ""

    Public f As FontStyle = FontStyle.Regular
    Public font_name As String = "Aharoni"

    Public zadat_string As Boolean = False
    'karakteri i cifre koji su dozvoljeni za velicinu 6 X 6, za vece probleme potrebno je dodati karaktera tako da znakovi.lenght = n * m
    Public znakovi As Char() = "123456789abcdefghijklmnopqrstuvwxyz*"
    Public dozvoljeni_znakovi As Char()
    Public cifre As Char() = "123456789"
    Public b_slucajan As Integer  ' promjenljive sa prefiksom b_ koriste se u BackTracku
    Public b_rezultat_iz_zadatka As String = ""
    Public b_rezultat_iz_mat As String = ""
    Public b_spojeni As String = ""
    Public b_randomNum As New Random(Now.Millisecond)
    Public b_i As Integer = 0
    Public b_n As Integer = 0
    Public s_pokupi_brojeve_iz_matrice As String = ""
    Public s_pokupi_brojeve_iz_vrste As String = ""
    Public b_test As Integer = 0
    Public b_upis As Boolean = False
    Public b_novi As String = ""
    Public b_preostali As String = ""
    Public promjena_novo As Integer = 0
    Public promjena_staro As Integer = 0
    Public zadnji_radio As Integer = 0
    '   Public niz_boja() As Color
    Public aktuelna_boja As Color
    Public T_aktuelna_boja As Color = Color.Black

    ' **********************  STARTOVANJE IGRE ********************************************************** START
    Public Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'TextBox3.Text = "005009208080000003701200000820370410006592300073084062000001804400000020302800600"    ' easy
        TextBox3.Text = "0100000623096405000000094830004000000420307900000050007358000000010978049800000700"    ' mediym
        'TextBox3.Text = "106900000785000904000080010800060000009301800000040006030090000201000569000005702"    ' hard
        'TextBox3.Text = "000708002000050170095001000500100090070000040080007001000300280026040000300602000"    ' evil

        Dim i As Integer

        For i = 2 To 6
            ComboBox3.Items.Add(i)
            ComboBox4.Items.Add(i)
        Next

        ComboBox3.SelectedItem = 3
        ComboBox4.SelectedItem = 3
        n = n_malo * m_malo
        m = n_malo * m_malo
    End Sub

    Public Sub Podesi_Fontove()
        If (n_malo = 3 And m_malo = 2) Or (n_malo = 2 And m_malo = 3) Or (n_malo = 2 And m_malo = 2) Then
            t_font = 28
        End If
        If n_malo = 3 And m_malo = 3 Then
            t_font = 18
        End If
        If (n_malo = 3 And m_malo = 4) Or (n_malo = 4 And m_malo = 3) Or (n_malo = 4 And m_malo = 4) Then
            t_font = 14
            '     f = FontStyle.Bold
        End If
        If (n_malo = 4 And m_malo = 5) Or (n_malo = 5 And m_malo = 4) Then
            t_font = 10
            f = FontStyle.Bold
        End If
        If (n_malo = 5 And m_malo = 5) Then
            t_font = 8
            f = FontStyle.Bold
        End If
        If (n_malo = 6 And m_malo = 5) Or (n_malo = 5 And m_malo = 6) Then
            t_font = 8
            f = FontStyle.Bold
        End If
        If (n_malo = 3 And m_malo >= 5) Or (n_malo >= 5 And m_malo = 3) Then
            t_font = 12
            f = FontStyle.Bold
        End If
        If (n_malo = 2 And m_malo >= 5) Or (n_malo >= 5 And m_malo = 2) Then
            t_font = 14
            '   f = FontStyle.Bold
        End If
        If (n_malo = 6 And m_malo = 6) Then
            t_font = 7
            f = FontStyle.Bold
        End If
    End Sub

    Public Sub Setuj_Velicine()

        n_malo = CInt(ComboBox3.SelectedItem)
        m_malo = CInt(ComboBox4.SelectedItem)
        n = n_malo * m_malo
        m = n_malo * m_malo

        If (n_malo >= 5) And (m_malo >= 4) Then
            t_velicina = 700 / (n_malo * m_malo + 1)
        Else
            t_velicina = 550 / (n_malo * m_malo + 1)
        End If
        '    t_velicina = 550 / (n_malo * m_malo + 1)

        preskok_m = m_malo
        preskok_n = n_malo - 1

        s = pretvori(TextBox3.Text)

        If s.Length >= n * m Then
            zadat_string = True
        Else
            zadat_string = False
        End If

        ReDim dozvoljeni_znakovi((n_malo * m_malo) - 1)
        Dim za_stampu As String = " "

        For i = 0 To n_malo * m_malo - 1
            dozvoljeni_znakovi(i) = znakovi(i)
            za_stampu = za_stampu & " " & znakovi(i)
        Next

        TextBox18.Text = dozvoljeni_znakovi
    End Sub

    ' **********************  END OF STARTOVANJE IGRE ************************************************ END
    ' **********************  FORMIRANJE MATRICA ******************************************************** START
    ' Kreira matricu textboxova, pozicionira ih, upisuje zadatak 
    ' Jednoznacni u malim matricama (mala matrica je matrica dimenzija 3 x 3)
    ' Prolazi kroz pocetne pozicije malih matrica ( (0,0), (0,3), (0,6), (3,0), (3,3), (3,6), (6,0)... ) 
    ' i poziva funkciju za provjeru da li postoje jednoznacno odredjeni brojevi za ove male matrice
    ' formira matricu texbox-ova, popunjava u odgovarajuce vrijednosti iz zadatka
    Public Sub Formiraj_Igru()
        '  Me.Size = MaximumSize
        '  Me.Location = New Point(0, 0)
        TextBox3.Text = pretvori(TextBox3.Text)
        ReDim t(n, m)
        ReDim mat(n, m)
        ReDim z(n, m)
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim x As Integer = 20
        Dim y As Integer = 125
        Dim br As Integer = 0

        For i = 0 To n - 1
            For j = 0 To m - 1
                If j = preskok_m Then
                    x += 3
                    preskok_m = preskok_m + m_malo
                End If
                t(i, j) = New TB
                t(i, j).t.Multiline = True
                '  t(i, j).t.BackColor = Color.GhostWhite
                t(i, j).t.Height = t_velicina
                t(i, j).t.Width = t_velicina
                '  odredivanje fonta

                t(i, j).t.Font = New System.Drawing.Font(font_name, t_font, f)
                '  t(i, j).t.ForeColor = Color.Red
                t(i, j).t.TextAlign = HorizontalAlignment.Center
                If zadat_string Then
                    If (s(br)) <> "0" And (s(br)) <> "." Then
                        '        t(i, j).t.ForeColor = T_aktuelna_boja
                        t(i, j).t.Text = s(br)
                        t(i, j).t.ReadOnly = True
                        t(i, j).t.BackColor = Color.White
                        mat(i, j) = s(br)
                    End If
                Else
                    '  t(i, j).t.ForeColor = T_aktuelna_boja
                    t(i, j).t.Text = ""

                End If
                br += 1
                t(i, j).t.Location = New Point(x, y)
                Me.Controls.Add(t(i, j).t)
                x += t_velicina
            Next
            x = 20
            If i = preskok_n Then
                y += 3
                preskok_n = preskok_n + n_malo
            End If
            preskok_m = m_malo
            y += t_velicina
        Next
        n_s = n
        m_s = m

        preskok_m = m_malo
        preskok_n = n_malo - 1

        ReDim k(n, m)
        i = 0
        j = 0
        x = 880
        y = 125
        br = 0
        For i = 0 To n - 1
            For j = 0 To m - 1
                If j = preskok_m Then
                    x += 3
                    preskok_m = preskok_m + m_malo
                End If

                k(i, j) = New Sudoku.TBkandidat
                k(i, j).t.Multiline = True
                k(i, j).t.Height = t_velicina
                k(i, j).t.Width = t_velicina

                '  k(i, j).t.BackColor = Color.Aqua
                '  Form1.k(i, j).t.Font = New System.Drawing.Font(font_name, t_font, f)
                k(i, j).t.TextAlign = HorizontalAlignment.Center
                br += 1
                k(i, j).t.Location = New Point(x, y)
                Me.Controls.Add(k(i, j).t)
                k(i, j).t.BringToFront()
                x += t_velicina
            Next
            x = 880
            If i = preskok_n Then
                y += 3
                preskok_n = preskok_n + n_malo
            End If
            preskok_m = m_malo
            y += t_velicina
        Next
        '  Label1.BackColor = k(0, 0).t.BackColor
    End Sub

    ' Kreira matricu zadatka na osnovu globalnog stinga zadat_string u kome se postavlja zadatak
    Public Sub Formiraj_Matricu_Zadatka_z()

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim br As Integer = 0

        For i = 0 To n - 1
            For j = 0 To m - 1
                If zadat_string Then
                    z(i, j) = s(br)
                End If
                br += 1
            Next
        Next
    End Sub

    Public Sub Formiraj_Mat()

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim br As Integer = 0

        For i = 0 To n - 1
            For j = 0 To m - 1
                If t(i, j).t.Text = "" Then
                    mat(i, j) = 0
                Else
                    mat(i, j) = t(i, j).t.Text
                End If
            Next
        Next
    End Sub

    ' ********************** END OF FORMIRANJE MATRICA ********************************************** END
    ' ********************** PRIKAZIVANJE MATRICA ****************************************************** START
    Public Sub Prikazi_Matricu_Zadatka_z()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim br As Integer = 0
        Dim s As String = ""

        For i = 0 To n - 1
            For j = 0 To m - 1
                If zadat_string Then
                    s = s & "  " & z(i, j).ToString

                End If
                br += 1
            Next
            s = s '& vbCrLf
        Next

    End Sub

    ' Prikazuje stanje u texboxovima
    Public Sub Prikazi_Matricu_TextBox_ova_t()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim s As String = ""

        For i = 0 To n - 1
            For j = 0 To m - 1
                If t(i, j).t.Text.Length = 0 Then
                    s = s & "  " & "0"
                Else
                    If t(i, j).t.Text.Length = 2 Then
                        s = s & t(i, j).t.Text.ToString

                    Else
                        s = s & "  " & t(i, j).t.Text.ToString
                    End If

                End If
            Next
            s = s & vbCrLf
        Next
        '  TextBox6.Text = s
    End Sub

    Public Sub Prikazi_Matricu_mat()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim s As String = ""

        For i = 0 To n - 1
            For j = 0 To m - 1
                If mat(i, j).Length > 1 Then
                    s = s & " " & mat(i, j)      '?
                Else
                    s = s & "  " & mat(i, j)
                End If
            Next
            s = s & vbCrLf
        Next
    End Sub

    Public Sub Prikazi()
        Dim i As Integer = 0
        Dim j As Integer = 0

        For i = 0 To n - 1
            For j = 0 To m - 1
                t(i, j).t.BringToFront()
                t(i, j).t.Refresh()
            Next
        Next
    End Sub

    Public Sub Oboji_Matricu_Zadataka()
        Dim ii As Integer = 0
        Dim jj As Integer = 0
        ' kroz kolonu
        For ii = 0 To n - 1
            For jj = 0 To m - 1
                t(ii, jj).t.BackColor = Color.White
                '   t(ii, jj).t.ForeColor = Color.Red
            Next
        Next
    End Sub

    Public Sub Oboji_Matricu_Kandidata()
        Dim ii As Integer = 0
        Dim jj As Integer = 0
        ' kroz kolonu
        For ii = 0 To n - 1
            For jj = 0 To m - 1
                k(ii, jj).t.BackColor = Color.Aqua
                k(ii, jj).t.ForeColor = Color.Black
                '    k(ii, jj).l(ii).BackColor = Color.Red
                '    k(ii, jj).l(ii).Height = 50
                '    k(ii, jj).l(ii).Width = 50
                '    k(ii, jj).l(ii).BringToFront()
            Next
        Next
    End Sub

    Public Sub Prepisu_Iz_T_U_Z()
        Dim i As Integer = 0
        Dim j As Integer = 0
        ' kroz kolonu
        For i = 0 To n - 1
            For j = 0 To m - 1
                If t(i, j).t.Text <> "" Then
                    '   t(ii, jj).t.ForeColor = T_aktuelna_boja
                    z(i, j) = t(i, j).t.Text
                Else
                    z(i, j) = "0"
                End If
            Next
        Next
    End Sub

    ' ********************** END OF PRIKAZIVANJE MATRICA ******************************************** END
    ' ***************    KUPLJENJE PODATAKA  *********************************************************** START
    ' kupi sve brojeve koji su upisani u matricu pocev od pozicije mat(nn,mm), ova pozicija predstavlja pocetno polje male matrice 
    ' nn,mm se predaju kao parametri pri pozivu funkcije
    ' ako je z_mat = 1 kupi podatke iz z(,) matrice (matrica zadata + jednoznacni), 
    ' ako je z_mat = 0 kupi iz mat(,) matrice (matrica u kojoj se trazi resenje, radi backtrack...) 
    Public Function Pokupi_Brojeve_Iz_Matrice(ByVal nn As Integer, ByVal mm As Integer, ByVal z_mat As Integer) As String

        Dim ii As Integer = 0
        Dim jj As Integer = 0
        Dim s As String = ""
        s_pokupi_brojeve_iz_matrice = ""

        For ii = nn To nn + n_malo - 1
            For jj = mm To mm + m_malo - 1
                '  If (((t(ii, jj).t.Text) <> "0") And (t(ii, jj).t.Text <> "")) Then  '?
                '  s = s & t(ii, jj).t.Text
                '  End If
                Select Case z_mat
                    Case 0
                        If ((mat(ii, jj) <> "0") And (mat(ii, jj) <> "")) Then  '?
                            s_pokupi_brojeve_iz_matrice = s_pokupi_brojeve_iz_matrice & mat(ii, jj)
                        End If
                    Case 1
                        If ((z(ii, jj) <> "0") And (z(ii, jj) <> "")) Then  '?
                            s_pokupi_brojeve_iz_matrice = s_pokupi_brojeve_iz_matrice & z(ii, jj)
                        End If
                    Case 2
                        If (t(ii, jj).t.Text <> "") Then  '?
                            s_pokupi_brojeve_iz_matrice = s_pokupi_brojeve_iz_matrice & t(ii, jj).t.Text
                        End If
                    Case Else

                End Select
            Next
        Next
        Return s_pokupi_brojeve_iz_matrice
    End Function

    ' kupi sve brojeve koji su upisani u koloni j, od 0 do granica - do koje pozicije da ide, 
    '  0 - iz  mat(,) 
    '  1 - z(,)    
    '  2 - t(,)
    Public Function Pokupi_Brojeve_Iz_Kolone(ByVal j As Integer, ByVal granica As Integer, ByVal z_mat As Integer) As String
        Dim ii As Integer = 0
        Dim s As String = ""

        For ii = 0 To granica - 1
            Select Case z_mat
                Case 0
                    If (mat(ii, j) <> "0") Then
                        s = s & mat(ii, j)
                    End If
                Case 1
                    If (z(ii, j) <> "0") Then
                        s = s & z(ii, j)
                    End If
                Case 2
                    If t(ii, j).t.Text <> "" Then  '? 'odje puca kolona
                        s = s & t(ii, j).t.Text
                    End If
                Case Else
            End Select
        Next
        Return s
    End Function

    '  kupi sve brojeve koji su upisani u vrsti i, granica - do koje pozicije da ide, 
    '  0 - iz mat(,)   
    '  1 - z(,) 
    '  2 - t(,)
    Public Function Pokupi_Kandidate_Iz_Vrste(ByVal i As Integer) As String
        Dim kandidati As Char() = ""
        Dim j As Integer = 0

        For j = 0 To n - 1
            If t(i, j).t.Text = "" Then
                kandidati = kandidati & k(i, j).t.Text
            End If
        Next
        Return kandidati
    End Function

    Public Function Pokupi_Kandidate_Iz_Kolone(ByVal j As Integer) As String
        Dim kandidati As Char() = ""
        Dim i As Integer = 0

        For i = 0 To n - 1
            If t(i, j).t.Text = "" Then
                kandidati = kandidati & k(i, j).t.Text
            End If
        Next
        Return kandidati
    End Function

    Public Function Pokupi_Kandidate_Iz_Male_Matrice(ByVal i As Integer, ByVal j As Integer) As String
        Dim kandidati As String = ""
        Dim ii As Integer = 0
        Dim jj As Integer = 0

        '  MsgBox(Nadji_RED_Pocetnog_Polja_Male_Matrice(i) & " " & Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j) & "  n=" & n_malo & "  m=" & m_malo)
        For ii = Nadji_RED_Pocetnog_Polja_Male_Matrice(i) To Nadji_RED_Pocetnog_Polja_Male_Matrice(i) + n_malo - 1
            For jj = Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j) To Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j) + m_malo - 1
                If t(ii, jj).t.Text = "" Then
                    kandidati = kandidati & k(ii, jj).t.Text
                End If
            Next
        Next
        Return kandidati
    End Function

    Public Function Pokupi_Brojeve_Iz_Vrste(ByVal i As Integer, ByVal granica As Integer, ByVal z_mat As Integer) As String
        Dim jj As Integer = 0
        Dim s As String = ""
        s_pokupi_brojeve_iz_vrste = ""

        For jj = 0 To granica - 1
            Select Case z_mat
                Case 0
                    If mat(i, jj) <> "0" Then
                        s_pokupi_brojeve_iz_vrste = s_pokupi_brojeve_iz_vrste & mat(i, jj)   '?
                    End If
                Case 1
                    If z(i, jj) <> "0" Then
                        s_pokupi_brojeve_iz_vrste = s_pokupi_brojeve_iz_vrste & z(i, jj)   '?
                    End If
                Case 2
                    If t(i, jj).t.Text <> "" Then  '?
                        s_pokupi_brojeve_iz_vrste = s_pokupi_brojeve_iz_vrste & t(i, jj).t.Text    ' odje puca
                    End If
            End Select
        Next
        Return s_pokupi_brojeve_iz_vrste
    End Function

    Public Function Pokupi_Brojeve_Iz_Vrste_Do_Polja(ByVal i As Integer, ByVal j As Integer) As String
        s_pokupi_brojeve_iz_vrste = ""

        If j = 0 Then
            Return mat(i, 0)
        End If
        While j > 0
            j = j - 1
            s_pokupi_brojeve_iz_vrste = s_pokupi_brojeve_iz_vrste & mat(i, j)
        End While
        Return s_pokupi_brojeve_iz_vrste
    End Function

    Public Function Pokupi_Brojeve_Iz_Kolone_Do_Polja(ByVal i As Integer, ByVal j As Integer) As String
        s_pokupi_brojeve_iz_vrste = ""

        If i = 0 Then
            Return mat(0, j)
        End If

        While i > 0
            i = i - 1
            s_pokupi_brojeve_iz_vrste = s_pokupi_brojeve_iz_vrste & mat(i, j)
        End While

        Return s_pokupi_brojeve_iz_vrste

    End Function

    Public Function Nadji_RED_Pocetnog_Polja_Male_Matrice(ByVal ii As Integer) As Integer
        Dim i As Integer = 0

        While i < n
            If ii >= i And ii <= i + n_malo - 1 Then
                Return i
            End If
            i += n_malo
        End While
        Return i
    End Function

    Public Function Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(ByVal jj As Integer) As Integer
        Dim j As Integer = 0

        While jj < m
            If jj >= j And jj <= j + m_malo - 1 Then
                Return j
            End If
            j += m_malo
        End While
        Return j
    End Function

    ' ***************   END OF KUPLJENJE POKADAKA  ************************************************** END
    ' ***************   RAD SA STRINGOVIMA  ************************************************************ START
    Public Function Ukloni_Nule(ByVal stari As String) As String
        Dim novi As String = ""
        Dim i As Integer = 0

        For i = 0 To stari.Length - 1
            If stari(i) <> "0" Then
                novi = stari(i) & novi
            End If
        Next
        Return novi
    End Function

    Public Function Ima_Li_Istih(ByVal s As String) As Boolean
        Dim i As Integer = 0
        Dim j As Integer = 0

        For i = 0 To s.Length - 2
            For j = i + 1 To s.Length - 1
                If s(i) = s(j) Then
                    Return True
                End If
            Next
        Next
        Return False
    End Function

    Public Function Ukloni_Iste(ByVal stari As String) As String
        b_novi = ""
        Dim i As Integer = 0

        If stari.Length > 0 Then
            For i = 0 To stari.Length - 1
                If Not b_novi.Contains(stari(i)) Then
                    b_novi = b_novi & stari(i)
                End If
            Next
        End If
        Return b_novi
    End Function

    Public Function Ukloni_Iste_Iz_Dva_Stringa(ByVal veci As String, ByVal manji As String) As String
        Dim novi As String = ""
        Dim i As Integer = 0

        For i = 0 To veci.Length - 1
            If Not manji.Contains(veci(i)) Then
                novi = novi & veci(i)
            End If
        Next
        For i = 0 To manji.Length - 1
            If Not veci.Contains(manji(i)) Then
                novi = novi & manji(i)
            End If
        Next
        Return novi
    End Function

    Public Function Ukloni_Karaktere_Stringa1_Iz_Striga2(ByVal str1 As String, ByVal str2 As String)
        Dim s As String = str2
        Dim i As Integer = 0
        For i = 0 To str1.Length - 1
            s = s.Replace(str1(i), "")
        Next
        Return s
    End Function

    ' vraca jedan karakter koji nedostaje u stringu stari u odnosu na dozvoljeni
    Public Function Koji_Su_Jedinstveni(ByVal s As String) As String
        Dim jedinstveni As String = ""
        Dim jeste As Boolean = True
        Dim i As Integer = 0
        Dim j As Integer = 0

        For i = 0 To s.Length - 1
            jeste = False
            For j = 0 To s.Length - 1
                If i <> j Then
                    If s(i) = s(j) Then
                        jeste = True
                    End If
                End If
            Next
            If jeste = False Then
                jedinstveni = jedinstveni & s(i)
            End If
        Next
        Return jedinstveni
    End Function

    Public Function Koji_Nedostaje(ByVal stari As String) As String
        Dim n As String = ""
        Dim i As Integer = 0

        For i = 0 To dozvoljeni_znakovi.Length - 1
            If Not stari.Contains(dozvoljeni_znakovi(i)) Then
                Return dozvoljeni_znakovi(i)
            End If
        Next
        Return n
    End Function

    Public Function Svi_Koji_Nedostaju(ByVal stari As String) As String
        Dim n As String = ""
        Dim i As Integer = 0

        For i = 0 To dozvoljeni_znakovi.Length - 1
            If Not stari.Contains(dozvoljeni_znakovi(i)) Then
                n = n & dozvoljeni_znakovi(i)
            End If
        Next
        Return n
    End Function

    ' vraca ascii - 87 , za  a vratice 10, b = 11, c = 12 ...
    Public Function U_Broj(ByVal c As Char) As Integer
        Return Asc(c) - 87
    End Function

    ' vraca (i + 87)toChar , za  10 vratice a, 11 = b, 12 = c ...
    Public Function U_Char(ByVal i As Integer) As Char
        Return Convert.ToChar(i + 87)
    End Function

    ' ***************  END OF RAD SA STRINGOVIMA  *************************************************** END
    ' ***************   RAD SA JEDNOZNACNIM  *********************************************************** START
    Public Sub Dodaj_Jednoznacne_U_Matricu_Zadatka()
        Dim i, j As Integer

        For i = 0 To n - 1
            For j = 0 To m - 1
                If mat(i, j) <> z(i, j) Then
                    z(i, j) = mat(i, j)
                    '    z(i, j) = mat(i, j)
                End If
            Next
        Next
    End Sub

    Public Sub Popuni_Jednoznacne()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim rez As String = ""
        Dim jednoznacni As String = ""
        Dim upisivano As Integer = 1

        While upisivano = 1
            upisivano = 0
            For i = 0 To n - 1
                For j = 0 To m - 1
                    If t(i, j).t.Text = "" Then
                        rez = Pokupi_Brojeve_Iz_Matrice(Nadji_RED_Pocetnog_Polja_Male_Matrice(i), Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j), 1) & Pokupi_Brojeve_Iz_Vrste(i, m, 1) & Pokupi_Brojeve_Iz_Kolone(j, n, 1)
                        rez = Ukloni_Iste(rez)
                        If rez.Length + 1 = dozvoljeni_znakovi.Length Then
                            jednoznacni = Koji_Nedostaje(rez)
                            mat(i, j) = Koji_Nedostaje(rez)        '?
                            ' t(i, j).t.ForeColor = T_aktuelna_boja
                            t(i, j).t.Text = mat(i, j)
                            z(i, j) = mat(i, j)
                            '  t(i, j).t.ReadOnly = True
                            t(i, j).t.BackColor = Color.Aqua
                            upisivano = 1
                        End If
                    End If
                    rez = ""
                Next
            Next
        End While
    End Sub

    Public Sub Popuni_Jednoznacne_Iz_BackTracka()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim rez As String = ""
        Dim jednoznacni As String = ""
        Dim upisivano As Integer = 1

        While upisivano = 1
            upisivano = 0
            For i = 0 To n - 1
                For j = 0 To m - 1
                    If t(i, j).t.Text = "" Then
                        rez = Pokupi_Brojeve_Iz_Matrice(Nadji_RED_Pocetnog_Polja_Male_Matrice(i), Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j), 1) & Pokupi_Brojeve_Iz_Vrste(i, m, 1) & Pokupi_Brojeve_Iz_Kolone(j, n, 1)
                        rez = Ukloni_Iste(rez)
                        If rez.Length + 1 = dozvoljeni_znakovi.Length Then
                            jednoznacni = Koji_Nedostaje(rez)
                            ' t(i, j).t.ForeColor = T_aktuelna_boja
                            t(i, j).t.Text = Koji_Nedostaje(rez)

                            '      t(i, j).t.BackColor = Color.Chocolate
                            upisivano = 1
                        End If
                    End If
                    rez = ""
                Next
            Next
        End While
    End Sub

    ' ***************  END OF RAD SA JEDNOZNACNIM  ************************************************** END
    ' ***************  SVASTA  ************************************************************************* START
    ' Prolazi kroz pocetna polja malih matrica i 
    'nn, mm pocetna polja malih matrica
    Public Sub Obilazak_Pocetnih_Polja_Malih_Matrica()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim br As Integer = 1

        While i < n
            While j < m
                ' t(i, j).t.Text = br         '?
                mat(i, j) = br
                j += m_malo
                br += 1
            End While
            j = 0
            i += n_malo
        End While
    End Sub

    ' na osnovu parametra sa kojim se poziva nalazi Pocetno Polje Malih Matrica
    Public Function pretvori(ByVal s As String) As String
        Dim novi(CInt(s.Length) - 1) As Char
        Dim i As Integer = 0

        For i = 0 To s.Length - 1
            If s(i) = "." Then
                novi(i) = "0"
            Else
                novi(i) = s(i)
            End If
        Next
        Return novi
    End Function

    ' popunjava t() i z() istovremeno 
    Public Sub Obrisi()
        Panel1.BringToFront()
        Panel2.BringToFront()
    End Sub

    ' ***************  END OF SVASTA  *************************************************************** END
    ' ***************  BUTTON  ************************************************************************* START
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Setuj_Velicine()
        Podesi_Fontove()
        Obrisi()
        Formiraj_Igru()
        Prikazi()
        Formiraj_Matricu_Zadatka_z()
        '  Prikazi_Matricu_Zadatka_z()
        '  Prikazi_Matricu_Zadatka_z()
        Formiraj_Mat()
        '  Prikazi_Matricu_TextBox_ova_t()
        '  Prikazi_Matricu_mat()
        '  Prikazi_Matricu_mat()
        '  Prikazi_Matricu_Zadatka_z()
        '*********************** end
        kandidati()

    End Sub

    Public Function Ima_Li_Jednog() As Boolean
        Dim tac As Boolean = False
        Dim i As Integer
        Dim j As Integer

        For i = 0 To n - 1
            For j = 0 To m - 1
                If k(i, j).t.Text.Length = 1 And t(i, j).t.text = "" Then
                    k(i, j).t.ForeColor = Color.Red
                    k(i, j).t.Font = New System.Drawing.Font(font_name, 12, FontStyle.Bold)
                    k(i, j).t.BackColor = Color.Blue
                    tac = True
                End If
            Next
        Next
        Return tac
    End Function

    Public Sub Upisi_Jednog()
        Dim i As Integer
        Dim j As Integer
        For i = 0 To n - 1
            For j = 0 To m - 1
                If k(i, j).t.Text.Length = 1 And t(i, j).t.Text = "" Then
                    ' t(i, j).t.ForeColor = T_aktuelna_boja
                    t(i, j).t.Text = k(i, j).t.Text
                    '            k(i, j).t.Font = New System.Drawing.Font(font_name, 12, FontStyle.Bold)
                    '            k(i, j).t.BackColor = Color.Blue
                End If
            Next
        Next
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim ii As Integer = 0
        Dim jj As Integer = 0

        ' kroz kolonu
        For ii = 0 To n - 1
            For jj = 0 To m - 1
                If k(ii, jj).t.Text.Length = 1 And t(ii, jj).t.Text = "" Then
                    '   t(ii, jj).t.ForeColor = T_aktuelna_boja
                    t(ii, jj).t.Text = k(ii, jj).t.Text
                    t(ii, jj).t.BackColor = Color.Aqua
                    k(ii, jj).t.BackColor = Color.White
                End If
            Next
        Next
        '   kandidati()
        Smanji_Broj_Kandidata()
        If Ima_Li_Jednog() Then
            Button4.BackColor = Color.Blue
            '   Button4.Width = 140
            '   Button4.Height = 30
            Button4.Focus()
        Else
            Button9.BackColor = Color.Coral
            Button9.Focus()
            Button4.BackColor = Color.Empty
            '   Button4.Width = 100
            '   Button4.Height = 22
            If zadnji_radio <> 2 Then
                Oboji_Matricu_Kandidata()
                Oboji_Matricu_Zadataka()
            End If
        End If
        '    Popuni_Jednoznacne()
        '    Prikazi_Matricu_mat()
        '    Dodaj_Jednoznacne_U_Matricu_Zadatka()
        '    Prikazi_Matricu_Zadatka_z()
    End Sub

    ' *************** END OF BUTTON  *************************************************************** END
    Public Function Jedan_U_Kocki_Za_Svako_Polje(ByVal i As Integer, ByVal j As Integer) As Boolean
        b_rezultat_iz_zadatka = Pokupi_Brojeve_Iz_Matrice(Nadji_RED_Pocetnog_Polja_Male_Matrice(i), Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j), 0)
        If b_rezultat_iz_zadatka.Length = (n_malo * m_malo) - 1 Then
            ' t(i, j).t.ForeColor = T_aktuelna_boja
            t(i, j).t.Text = Koji_Nedostaje(b_rezultat_iz_zadatka)
            mat(i, j) = Koji_Nedostaje(b_rezultat_iz_zadatka)
            ' MsgBox(b_rezultat_iz_zadatka & " len=" & b_rezultat_iz_zadatka.Length & "manji je za 1 ")
            Return True
        End If

        '        MsgBox(b_rezultat_iz_zadatka & " len=" & b_rezultat_iz_zadatka.Length)
        Return False ' ako nije nista upisao
    End Function

    Public Sub Provjera()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim s As String = ""
        Dim greska As String = ""

        For i = 0 To n - 1
            For j = 0 To m - 1
                s = Pokupi_Brojeve_Iz_Matrice(Nadji_RED_Pocetnog_Polja_Male_Matrice(i), Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j), 2)
                If Ima_Li_Istih(s) Then
                    greska = greska & vbCrLf & "(" & i & " , " & j & ")"
                End If
                s = Pokupi_Brojeve_Iz_Vrste(i, m, 2)
                If Ima_Li_Istih(s) Then
                    greska = greska & vbCrLf & "(" & i & " , " & j & ")"
                End If
                s = Pokupi_Brojeve_Iz_Kolone(j, n, 2)
                If Ima_Li_Istih(s) Then
                    greska = greska & vbCrLf & "(" & i & " , " & j & ")"
                End If
            Next
        Next
        If greska.Length > 0 Then
            MsgBox("Nije uredu matrica " & vbCrLf & greska)
        End If
    End Sub

    Public Function Ispunjava_Li_Uslove(ByVal i As Integer, ByVal j As Integer, ByVal kandidat As String) As Boolean
        Dim pokupljeni As String = Pokupi_Brojeve_Iz_Matrice(Nadji_RED_Pocetnog_Polja_Male_Matrice(i), Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j), 0) & Pokupi_Brojeve_Iz_Vrste(i, m, 0) & Pokupi_Brojeve_Iz_Kolone(j, n, 0)

        If pokupljeni.Contains(kandidat) Then
            Return False
        Else
            Return True
        End If
    End Function

    ' Zadovoljava_Li_Uslove provjerava po t(,) matrici(matrici textboxova koji su prikazani na formi)
    Public Function Zadovoljava_Li_Uslove(ByVal i As Integer, ByVal j As Integer, ByVal kandidat As String) As Boolean
        Dim ii As Integer = 0
        Dim jj As Integer = 0

        ' kroz kolonu
        For ii = 0 To m - 1
            If t(ii, j).t.Text <> "" Then
                If t(ii, j).t.Text = kandidat Then
                    Return False
                End If
            End If
        Next
        ' kroz vrstu
        For jj = 0 To n - 1
            If t(i, jj).t.Text <> "" Then
                If t(i, jj).t.Text = kandidat Then
                    Return False
                End If
            End If
        Next
        ' kroz malu matricu u kojoj je kandidat
        Dim red As Integer = Nadji_RED_Pocetnog_Polja_Male_Matrice(i)
        Dim kolona As Integer = Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j)
        For ii = red To n_malo - 1 + red
            For jj = kolona To m_malo - 1 + kolona
                If t(ii, jj).t.Text <> "" Then
                    If t(ii, jj).t.Text = kandidat Then
                        Return False
                    End If
                End If
            Next
        Next
        Return True
    End Function

    Public Sub kandidati()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim preostali As String = ""

        For i = 0 To n - 1
            For j = 0 To m - 1
                If t(i, j).t.Text = "" Then
                    preostali = Pokupi_Brojeve_Iz_Kolone(j, m, 2) & Pokupi_Brojeve_Iz_Vrste(i, n, 2)
                    preostali = preostali & Pokupi_Brojeve_Iz_Matrice(Nadji_RED_Pocetnog_Polja_Male_Matrice(i), Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j), 2)
                    preostali = Ukloni_Iste(preostali)
                    preostali = Svi_Koji_Nedostaju(preostali)
                    k(i, j).t.Text = preostali
                    '       k(i, j).t.BackColor = aktuelna_boja
                Else
                    k(i, j).t.Font = New System.Drawing.Font(font_name, t_font, f)
                    k(i, j).t.TextAlign = HorizontalAlignment.Center
                    '   k(i, j).t.ForeColor = Color.Red
                    k(i, j).t.Text = t(i, j).t.Text

                    '    k(i, j).t.BackColor = aktuelna_boja
                End If
            Next
        Next
    End Sub

    Public Sub Smanji_Broj_Kandidata()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim preostali As String = ""
        Dim ned As String = ""
        Dim isti As String = ""
        Dim pokupljeni As String = ""
        Dim za_prikaz As String = ""

        For i = 0 To n - 1
            For j = 0 To m - 1
                If t(i, j).t.Text = "" Then
                    preostali = Pokupi_Brojeve_Iz_Vrste(i, n, 2) & Pokupi_Brojeve_Iz_Kolone(j, m, 2)
                    preostali = preostali & Pokupi_Brojeve_Iz_Matrice(Nadji_RED_Pocetnog_Polja_Male_Matrice(i), Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j), 2)
                    pokupljeni = preostali
                    '     preostali = preostali & Pokupi_Kandidate_Iz_Vrste(i) & Pokupi_Kandidate_Iz_Kolone(j) & Pokupi_Kandidate_Iz_Male_Matrice(Nadji_RED_Pocetnog_Polja_Male_Matrice(i), Nadji_KOLONU_Pocetnog_Polja_Male_Matrice(j))
                    preostali = Ukloni_Iste(preostali)
                    isti = preostali

                    preostali = Svi_Koji_Nedostaju(preostali)
                    ned = preostali

                    If j = 4 And i = n - 1 Then
                        za_prikaz = "pok= " & pokupljeni & "  isti= " & isti & "  ned= " & ned & "  i= " & i & "j= " & j
                    End If
                    ' obavezno >=
                    If (k(i, j).t.Text.Length >= preostali.Length) And (preostali.Length >= 1) Then
                        k(i, j).t.Text = preostali
                        '    k(i, j).t.BackColor = aktuelna_boja
                    End If
                Else
                    k(i, j).t.Font = New System.Drawing.Font(font_name, t_font, f)
                    k(i, j).t.TextAlign = HorizontalAlignment.Center
                    '   k(i, j).t.ForeColor = Color.Red
                    k(i, j).t.Text = t(i, j).t.Text
                    '    k(i, j).t.BackColor = aktuelna_boja
                End If
            Next
        Next
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        TextBox3.Text = pretvori(TextBox3.Text)
    End Sub

    Public Function Nadji_Red_Prvog_Paraznog_Polja() As Integer
        Dim min = dozvoljeni_znakovi.Length
        Dim index As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0

        For i = 0 To n - 1
            For j = 0 To m - 1
                If (t(i, j).t.Text = "") Then
                    Return i
                End If
            Next
        Next
    End Function

    Public Function Nadji_Kolonu_Prvog_Paraznog_Polja() As Integer
        Dim min = dozvoljeni_znakovi.Length
        Dim index As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0

        For i = 0 To n - 1
            For j = 0 To m - 1
                If (t(i, j).t.Text = "") Then
                    Return j
                End If
            Next
        Next
    End Function

    Public Function Da_Li_Je_Resiva() As Boolean
        Dim prvo_i As Integer = Nadji_Red_Prvog_Paraznog_Polja()
        Dim prvo_j As Integer = Nadji_Kolonu_Prvog_Paraznog_Polja()
        Prepisu_Iz_T_U_Z()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim br As Integer = 0
        Dim preostali As String = ""
        Dim trenutna_pozicija As Integer = 0
        Dim sledeca_pozicija As Integer = 0

        Dim upis As Boolean = False
        Dim nn As Integer = 0

        While br < n * m

            If j = m Then
                i += 1
                j = 0
            End If
            If j < 0 Then
                j = m - 1
                i -= 1
            End If

            If z(i, j) = "0" Then
                preostali = k(i, j).t.Text
                If preostali.Length = 1 Then
                    '   ' t(i, j).t.ForeColor = T_aktuelna_boja
                    t(i, j).t.Text = preostali(0)
                    If CheckBox1.Checked Then
                        t(i, j).t.Refresh()                ' refresh moze se stavit pod komentare zbog brzine 
                    End If
                    j += 1
                    br += 1
                Else

                    If t(i, j).t.Text = "" Then
                        trenutna_pozicija = -1
                    Else
                        trenutna_pozicija = CInt(preostali.IndexOf(t(i, j).t.Text))
                    End If
                    sledeca_pozicija = trenutna_pozicija + 1
                    If i = prvo_i And j = prvo_j And sledeca_pozicija + 1 > preostali.Length Then
                        Return False
                    End If
                    upis = False

                    While ((upis = False) And (preostali.Length > sledeca_pozicija))
                        If Zadovoljava_Li_Uslove(i, j, preostali(sledeca_pozicija)) Then
                            '    ' t(i, j).t.ForeColor = T_aktuelna_boja
                            t(i, j).t.Text = preostali(sledeca_pozicija)
                            If CheckBox1.Checked Then
                                t(i, j).t.Refresh()
                            End If
                            upis = True
                        End If
                        sledeca_pozicija += 1
                    End While

                    If upis = True Then                   ' ako je nadjen odgovarajuci
                        j += 1
                        br += 1                           ' nastavi dalje
                    Else
                        '  ' t(i, j).t.ForeColor = T_aktuelna_boja
                        t(i, j).t.Text = ""
                        If CheckBox1.Checked Then
                            t(i, j).t.Refresh()
                        End If

                        j -= 1
                        br -= 1
                        If j < 0 Then
                            j = m - 1
                            i -= 1
                        End If

                        If z(i, j) <> "0" Then
                            While z(i, j) <> "0"               ' ako nije nadjen odgovarajuci vrati se nazad
                                j -= 1
                                br -= 1
                                If j < 0 Then
                                    j = m - 1
                                    i -= 1
                                End If
                            End While
                        End If
                        '    back(i, j, br)

                    End If
                End If
            Else
                br += 1
                j += 1
            End If
        End While
        Return True
    End Function

    Public Sub back(ByVal i As Integer, ByVal j As Integer, ByVal br As Integer)
        Dim preostali As String = ""
        Dim trenutna_pozicija As Integer = 0
        Dim sledeca_pozicija As Integer = 0
        Dim upis As Boolean = False
        Dim nn As Integer = 0

        If br = n * m Then
            Return
        End If
        If j = m Then
            i += 1
            j = 0
        End If
        If j < 0 Then
            j = m - 1
            i -= 1
        End If

        If z(i, j) = "0" Then
            preostali = k(i, j).t.Text
            If preostali.Length = 1 Then
                ' t(i, j).t.ForeColor = T_aktuelna_boja

                t(i, j).t.Text = preostali(0)
                back(i, j + 1, br + 1)
            Else

                If t(i, j).t.Text = "" Then
                    trenutna_pozicija = -1
                Else
                    trenutna_pozicija = CInt(preostali.IndexOf(t(i, j).t.Text))
                End If
                sledeca_pozicija = trenutna_pozicija + 1

                upis = False

                While ((upis = False) And (preostali.Length > sledeca_pozicija))
                    If Zadovoljava_Li_Uslove(i, j, preostali(sledeca_pozicija)) Then
                        ' t(i, j).t.ForeColor = T_aktuelna_boja
                        t(i, j).t.Text = preostali(sledeca_pozicija)

                        upis = True
                    End If
                    sledeca_pozicija += 1
                End While

                If upis = True Then                   ' ako je nadjen odgovarajuci
                    back(i, j + 1, br + 1)            ' nastavi dalje
                Else
                    ' t(i, j).t.ForeColor = T_aktuelna_boja
                    t(i, j).t.Text = ""
                    j -= 1
                    br -= 1
                    If j < 0 Then
                        j = m - 1
                        i -= 1
                    End If

                    If z(i, j) <> "0" Then
                        While z(i, j) <> "0"               ' ako nije nadjen odgovarajuci vrati se nazad
                            j -= 1
                            br -= 1
                            If j < 0 Then
                                j = m - 1
                                i -= 1
                            End If
                        End While
                    End If
                    back(i, j, br)
                End If
            End If
        Else
            back(i, j + 1, br + 1)
        End If
    End Sub

    Public Sub it()

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim br As Integer = 0
        Dim preostali As String = ""
        Dim trenutna_pozicija As Integer = 0
        Dim sledeca_pozicija As Integer = 0
        Dim upis As Boolean = False
        Dim nn As Integer = 0

        While br < n * m

            If j = m Then
                i += 1
                j = 0
            End If
            If j < 0 Then
                j = m - 1
                i -= 1
            End If

            If z(i, j) = "0" Then
                preostali = k(i, j).t.Text.ToString

                If preostali.Length = 1 Then
                    '   ' t(i, j).t.ForeColor = T_aktuelna_boja
                    t(i, j).t.Text = preostali(0)
                    If CheckBox1.Checked Then
                        t(i, j).t.Refresh()  ' refresh moze se stavit pod komentare zbog brzine 
                    End If
                    j += 1
                    br += 1
                Else

                    If t(i, j).t.Text = "" Then
                        trenutna_pozicija = -1
                    Else
                        trenutna_pozicija = CInt(preostali.IndexOf(t(i, j).t.Text))
                    End If
                    sledeca_pozicija = trenutna_pozicija + 1

                    upis = False

                    While ((upis = False) And (preostali.Length > sledeca_pozicija))

                        If Zadovoljava_Li_Uslove(i, j, preostali(sledeca_pozicija)) Then
                            '    ' t(i, j).t.ForeColor = T_aktuelna_boja
                            t(i, j).t.Text = preostali(sledeca_pozicija)
                            If CheckBox1.Checked Then
                                t(i, j).t.Refresh()
                            End If
                            upis = True
                        End If
                        sledeca_pozicija += 1
                    End While

                    If upis = True Then                   ' ako je nadjen odgovarajuci
                        j += 1
                        br += 1                           ' nastavi dalje
                    Else
                        '  ' t(i, j).t.ForeColor = T_aktuelna_boja
                        t(i, j).t.Text = ""
                        If CheckBox1.Checked Then
                            t(i, j).t.Refresh()
                        End If

                        j -= 1
                        br -= 1
                        If j < 0 Then
                            j = m - 1
                            i -= 1
                        End If

                        If z(i, j) <> "0" Then
                            While z(i, j) <> "0"               ' ako nije nadjen odgovarajuci vrati se nazad
                                j -= 1
                                br -= 1
                                If j < 0 Then
                                    j = m - 1
                                    i -= 1
                                End If
                            End While
                        End If
                        '    back(i, j, br)

                    End If
                End If
            Else
                br += 1
                j += 1
            End If
        End While
    End Sub

    ' upisuje jednog preostalog kandidata u t()
    Public Sub jed()
        Dim i As Integer
        Dim j As Integer

        For i = 0 To n - 1
            For j = 0 To m - 1
                If k(i, j).t.Text.Length = 1 Then
                    '  t(i, j).t.ForeColor = T_aktuelna_boja
                    t(i, j).t.Text = k(i, j).t.Text
                    z(i, j) = k(i, j).t.Text
                End If
            Next
        Next
        Prikazi_Matricu_Zadatka_z()
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        jed()
        it()
    End Sub

    Public Sub Jedinstveni_U_Redovima_I_Kolonama()
        Dim s As String = ""
        Dim jed As String = ""
        Dim i As Integer
        Dim j As Integer

        For i = 0 To n - 1
            For j = 0 To m - 1
                If t(i, j).t.Text = "" Then
                    s = Pokupi_Kandidate_Iz_Vrste(i) '& Pokupi_Kandidate_Iz_Kolone(j)
                    If Koji_Su_Jedinstveni(s).Length = 1 And k(i, j).t.Text.Contains(Koji_Su_Jedinstveni(s)) And Zadovoljava_Li_Uslove(i, j, Koji_Su_Jedinstveni(s)) Then
                        jed = Koji_Su_Jedinstveni(s)
                        k(i, j).t.Text = jed
                        '      k(i, j).t.BackColor = aktuelna_boja
                        k(i, j).t.ForeColor = Color.Red
                        k(i, j).t.Font = New System.Drawing.Font(font_name, 12, FontStyle.Bold)
                        k(i, j).t.BackColor = Color.Brown
                    End If
                    s = Pokupi_Kandidate_Iz_Kolone(j) '& Pokupi_Kandidate_Iz_Kolone(j)
                    If Koji_Su_Jedinstveni(s).Length = 1 And k(i, j).t.Text.Contains(Koji_Su_Jedinstveni(s)) And Zadovoljava_Li_Uslove(i, j, Koji_Su_Jedinstveni(s)) Then
                        k(i, j).t.Text = Koji_Su_Jedinstveni(s)
                        k(i, j).t.ForeColor = Color.Red
                        k(i, j).t.Font = New System.Drawing.Font(font_name, 12, FontStyle.Bold)
                        k(i, j).t.BackColor = Color.Brown
                    End If
                End If
            Next
        Next
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        If Ima_Li_Jednog() Then
            Button4.BackColor = Color.Blue
            Button4.Focus()
            ' Button4.Width = 140
            ' Button4.Height = 30
            ' Button4.Focus()
            zadnji_radio = 1
        End If
    End Sub
    '!!!!
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        Jedinstveni_U_Redovima_I_Kolonama()
        If Ima_Li_Jednog() Then
            Button4.BackColor = Color.Empty
            ' Button4.Focus()
            ' Button4.Width = 140
            ' Button4.Height = 30
            Button4.Focus()
            zadnji_radio = 1
        End If
    End Sub

    Public Sub Parovi_Kroz_Vrste()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim l As Integer = 0
        Dim broj_sa_dva_kandidata As Integer = 0
        Dim postoje As Boolean = False
        Dim kolona_prvog As Integer = -1
        Dim kolona_drugog As Integer = -1
        Dim red_zajednicki As Integer = -1
        Dim couples As String = ""

        For i = 0 To n - 1
            For j = 0 To m - 2
                If k(i, j).t.Text.Length = 2 Then
                    For l = j + 1 To m - 1
                        If (k(i, l).t.Text.Length = 2) And (k(i, j).t.Text = k(i, l).t.Text) Then
                            kolona_drugog = l
                            kolona_prvog = j
                            red_zajednicki = i
                        End If
                    Next
                End If
            Next ' po kolonama
            If red_zajednicki <> -1 Then
                couples = k(red_zajednicki, kolona_prvog).t.Text
                '     Dim ii As Integer
                Dim jj As Integer
                For jj = 0 To m - 1
                    If (jj <> kolona_prvog) And (jj <> kolona_drugog) Then
                        If k(red_zajednicki, jj).t.Text.Contains(couples(0)) Or k(red_zajednicki, jj).t.Text.Contains(couples(1)) Then
                            k(red_zajednicki, jj).t.BackColor = Color.Yellow
                            k(red_zajednicki, jj).t.Text = Ukloni_Karaktere_Stringa1_Iz_Striga2(couples, k(red_zajednicki, jj).t.Text)
                        End If
                    End If
                Next
            End If
            red_zajednicki = -1
        Next ' po redovima
    End Sub

    Public Sub Parovi_Kroz_Kolone()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim l As Integer = 0
        Dim broj_sa_dva_kandidata As Integer = 0
        Dim postoje As Boolean = False
        Dim red_prvog As Integer = -1
        Dim red_drugog As Integer = -1
        Dim kolona_zajednicka As Integer = -1
        Dim couples As String = ""

        For j = 0 To m - 1
            For i = 0 To n - 2
                If k(i, j).t.Text.Length = 2 Then
                    For l = i + 1 To n - 1
                        If (k(l, j).t.Text.Length = 2) And (k(i, j).t.Text = k(l, j).t.Text) Then
                            red_drugog = l
                            red_prvog = i
                            kolona_zajednicka = j
                        End If
                    Next
                End If
                ''''''''''''''''
                If kolona_zajednicka <> -1 Then
                    couples = k(red_prvog, kolona_zajednicka).t.Text
                    Dim ii As Integer
                    For ii = 0 To n - 1
                        If (ii <> red_prvog) And (ii <> red_drugog) Then
                            If k(ii, kolona_zajednicka).t.Text.Contains(couples(0)) Or k(ii, kolona_zajednicka).t.Text.Contains(couples(1)) Then
                                k(ii, kolona_zajednicka).t.BackColor = Color.Yellow
                                k(ii, kolona_zajednicka).t.Text = Ukloni_Karaktere_Stringa1_Iz_Striga2(couples, k(ii, kolona_zajednicka).t.Text)
                            End If
                        End If
                    Next
                    red_drugog = -1
                    red_prvog = -1
                    kolona_zajednicka = -1
                End If
                ''''''''''''''''''
            Next ' po kolonama
        Next ' po redovima
    End Sub

    Public Sub Parovi_U_Malim_Matricama()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim ii As Integer = 0
        Dim jj As Integer = 0
        Dim iii As Integer = 0
        Dim jjj As Integer = 0
        Dim br As Integer = 1
        Dim g As Integer = 0
        Dim r As Integer = 0
        Dim couples As String = ""

        While i < n
            While j < m
                '   t(i, j).t.Text = "br"
                For ii = i To i + n_malo - 1
                    For jj = j To j + m_malo - 1
                        If k(ii, jj).t.Text.Length = 2 Then
                            '    k(ii, jj).t.BackColor = Color.Yellow

                            For iii = i To i + n_malo - 1
                                For jjj = j To j + m_malo - 1
                                    If k(ii, jj).t.Text = k(iii, jjj).t.Text And ((iii <> ii) Or (jjj <> jj)) Then
                                        '    k(ii, jj).t.BackColor = Color.Yellow
                                        '    k(iii, jjj).t.BackColor = Color.Yellow
                                        couples = k(ii, jj).t.Text
                                        For g = i To i + n_malo - 1
                                            For r = j To j + m_malo - 1
                                                If (k(g, r).t.Text.Contains(couples(0)) Or k(g, r).t.Text.Contains(couples(1))) And (((ii <> g) Or (jj <> r)) And ((iii <> g) Or (jjj <> r))) Then
                                                    k(g, r).t.BackColor = Color.Yellow
                                                    k(g, r).t.Text = Ukloni_Karaktere_Stringa1_Iz_Striga2(couples, k(g, r).t.Text)
                                                End If
                                            Next
                                        Next
                                    End If
                                    '     couples = ""
                                Next
                            Next
                        End If
                    Next
                Next

                j += m_malo
                br += 1
            End While
            j = 0
            i += n_malo
        End While
    End Sub

    Public Sub Parovi()
        Parovi_Kroz_Vrste()
        Parovi_Kroz_Kolone()
        Parovi_U_Malim_Matricama()
    End Sub

    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        Parovi()
        Parovi_U_Malim_Matricama()
        Button4.BackColor = Color.Empty
        Button9.BackColor = Color.Empty

    End Sub

    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        Parovi_U_Malim_Matricama()
    End Sub

End Class