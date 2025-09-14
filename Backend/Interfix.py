import tkinter as tk
from tkinter import ttk, messagebox, simpledialog
import sqlite3
from datetime import datetime

# ========== BANCO DE DADOS ==========
class BancoDados:
    def __init__(self):
        self.conexao = sqlite3.connect('chamados.db')
        self.criar_tabelas()

    def criar_tabelas(self):
        cursor = self.conexao.cursor()
        # Tabela de usuarios
        cursor.execute('''CREATE TABLE IF NOT EXISTS usuarios (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        nome TEXT NOT NULL,
                        senha TEXT NOT NULL,
                        tipo TEXT NOT NULL DEFAULT 'user'
                      )''')
        
        # Tabela de chamados 
        cursor.execute('''CREATE TABLE IF NOT EXISTS chamados (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        titulo TEXT NOT NULL,
                        descricao TEXT NOT NULL,
                        status TEXT NOT NULL DEFAULT 'aberto',
                        data_abertura TEXT NOT NULL,
                        usuario_id INTEGER,
                        FOREIGN KEY (usuario_id) REFERENCES usuarios(id)
                      )''')
        self.conexao.commit()

    def inserir_usuario(self, nome, senha, tipo='user'):
        cursor = self.conexao.cursor()
        cursor.execute('INSERT INTO usuarios (nome, senha, tipo) VALUES (?, ?, ?)', (nome, senha, tipo))
        self.conexao.commit()
        return cursor.lastrowid

    def autenticar_usuario(self, nome, senha):
        cursor = self.conexao.cursor()
        cursor.execute('SELECT id, tipo FROM usuarios WHERE nome = ? AND senha = ?', (nome, senha))
        return cursor.fetchone()

    def inserir_chamado(self, titulo, descricao, usuario_id):
        data = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        cursor = self.conexao.cursor()
        cursor.execute('INSERT INTO chamados (titulo, descricao, status, data_abertura, usuario_id) VALUES (?, ?, ?, ?, ?)',
                      (titulo, descricao, 'aberto', data, usuario_id))
        self.conexao.commit()

    def buscar_chamados(self, status=None):
        cursor = self.conexao.cursor()
        query = 'SELECT * FROM chamados'
        if status:
            query += f' WHERE status = "{status}"'
        cursor.execute(query)
        return cursor.fetchall()

    def atualizar_chamado(self, chamado_id, status):
        cursor = self.conexao.cursor()
        cursor.execute('UPDATE chamados SET status = ? WHERE id = ?', (status, chamado_id))
        self.conexao.commit()

### ESSA PARTE DE BANCO DE DADOS NÃO É PARTE DO CODIGO FINAL, É APENAS UM "PROTOTIPO" 
### PARA TESTAR AS FUNÇÕES DO PROGRAMA.

# ========== INTERFACE GRÁFICA ==========
class TelaLogin:
    def __init__(self, root, bd):
        self.root = root
        self.bd = bd
        self.root.title("Login - Sistema Chamados")
        self.root.geometry("350x200")

        self.frame = ttk.Frame(self.root)
        self.frame.pack(pady=20)

        ttk.Label(self.frame, text="Usuário:").grid(row=0, column=0, padx=5, pady=5)
        self.entry_usuario = ttk.Entry(self.frame)
        self.entry_usuario.grid(row=0, column=1, padx=5, pady=5)

        ttk.Label(self.frame, text="Senha:").grid(row=1, column=0, padx=5, pady=5)
        self.entry_senha = ttk.Entry(self.frame, show="*")
        self.entry_senha.grid(row=1, column=1, padx=5, pady=5)

        ttk.Button(self.frame, text="Login", command=self.fazer_login).grid(row=2, columnspan=2, pady=10)

    def fazer_login(self):
        usuario = self.entry_usuario.get()
        senha = self.entry_senha.get()
        dados = self.bd.autenticar_usuario(usuario, senha)
        if dados:
            self.root.destroy()  # fecha a janela de login
            root = tk.Tk()
            TelaPrincipal(root, self.bd, dados[0], dados[1])  # Abre a tela principal
            root.mainloop()
        else:
            messagebox.showerror("Erro", "Usuário ou senha incorretos!")

class TelaPrincipal:
    def __init__(self, root, bd, usuario_id, tipo_usuario):
        self.root = root
        self.bd = bd
        self.usuario_id = usuario_id
        self.tipo_usuario = tipo_usuario

        self.root.title("Sistema de Chamados")
        self.root.geometry("800x600")

        self.criar_menu()
        self.criar_widgets()

    def criar_menu(self):
        menubar = tk.Menu(self.root)
        
        menu_chamados = tk.Menu(menubar, tearoff=0)
        menu_chamados.add_command(label="Novo Chamado", command=self.novo_chamado)
        menu_chamados.add_command(label="Todos Chamados", command=self.mostrar_chamados)
        menu_chamados.add_command(label="Meus Chamados", command=lambda: self.mostrar_chamados("meus"))
        
        if self.tipo_usuario == 'admin':
            menu_admin = tk.Menu(menubar, tearoff=0)
            menu_admin.add_command(label="Admin", command=self.mostrar_admin)
            menubar.add_cascade(label="Admin", menu=menu_admin)

        elif self.tipo_usuario == 'tecnico':
            menu_tecnico = tk.Menu(menubar, tearoff=0)
            menu_tecnico.add_command(label="Técnico", command=self.mostrar_tecnico)
            menubar.add_cascade(label="Técnico", menu=menu_tecnico)
        
        menubar.add_cascade(label="Chamados", menu=menu_chamados)
        menubar.add_command(label="Sair", command=self.sair)
        
        self.root.config(menu=menubar)

    def criar_widgets(self):
        self.frame = ttk.Frame(self.root)
        self.frame.pack(fill=tk.BOTH, expand=True, padx=10, pady=10)

        self.tree = ttk.Treeview(self.frame, columns=("ID", "Título", "Status", "Data"), show="headings")
        self.tree.heading("ID", text="ID")
        self.tree.heading("Título", text="Título")
        self.tree.heading("Status", text="Status")
        self.tree.heading("Data", text="Data Abertura")
        self.tree.pack(fill=tk.BOTH, expand=True)

        self.atualizar_lista_chamados()

    def atualizar_lista_chamados(self, filtro=None):
        for item in self.tree.get_children():
            self.tree.delete(item)
        
        if filtro == "meus":
            chamados = self.bd.buscar_chamados()  # pode ser modificado para filtrar por usuario_id
        else:
            chamados = self.bd.buscar_chamados()

        for chamado in chamados:
            self.tree.insert("", tk.END, values=chamado[:4])

    def novo_chamado(self):
        dialogo = tk.Toplevel(self.root)
        dialogo.title("Novo Chamado")

        ttk.Label(dialogo, text="Título:").grid(row=0, column=0, padx=5, pady=5)
        entry_titulo = ttk.Entry(dialogo)
        entry_titulo.grid(row=0, column=1, padx=5, pady=5)

        ttk.Label(dialogo, text="Descrição:").grid(row=1, column=0, padx=5, pady=5)
        entry_descricao = tk.Text(dialogo, width=30, height=5)
        entry_descricao.grid(row=1, column=1, padx=5, pady=5)

        def salvar_chamado():
            titulo = entry_titulo.get()
            descricao = entry_descricao.get("1.0", tk.END).strip()
            if titulo and descricao:
                self.bd.inserir_chamado(titulo, descricao, self.usuario_id)
                dialogo.destroy()
                self.atualizar_lista_chamados()
                messagebox.showinfo("Sucesso", "Chamado criado com sucesso!")
            else:
                messagebox.showerror("Erro", "Preencha todos os campos!")

        ttk.Button(dialogo, text="Salvar", command=salvar_chamado).grid(row=2, columnspan=2, pady=10)

    def mostrar_chamados(self, filtro=None):
        self.atualizar_lista_chamados(filtro)

    def mostrar_admin(self):
        messagebox.showinfo("Admin", "Área administrativa (em desenvolvimento)")

    def mostrar_tecnico(self):
        messagebox.showinfo("Técnico", "Área técnica (em desenvolvimento)")

    def sair(self):
        if confirmacao := messagebox.askyesno("Sair", "Deseja sair do sistema?"):
            if self.tipo_usuario == 'admin':
                self.bd.conexao.close()
            elif self.tipo_usuario == 'user':
                self.bd.conexao.commit()
            # Se mudou alguma coisa, confirma o commit
            if confirmacao:
                self.bd.conexao.commit()
            # Se é um técnico, tambem pode fechar a conexão
            # e não precisa confirmar o commit
            elif self.tipo_usuario == 'tecnico':
                self.bd.conexao.close()
            # Se não é nenhum dos casos, simplemente fecha a conexão
            elif self.tipo_usuario == 'tecnico':
                self.bd.conexao.close()
            else:
                self.bd.conexao.commit()
            self.root.destroy()
            

# ========== EXECUÇÃO ==========
if __name__ == "__main__":
    # Cria e inicializa o "banco de dados"
    bd = BancoDados()
    # Adiciona um usario, admin e um tecnico por defecto 
    try:
        bd.inserir_usuario("admin", "admin123", "admin")
        bd.inserir_usuario("user", "user123", "user")
        bd.inserir_usuario("tecnico", "tecnico123", "tecnico")
    except sqlite3.IntegrityError:
        pass  # Já existem usuários

    root = tk.Tk()
    TelaLogin(root, bd)
    root.mainloop()
