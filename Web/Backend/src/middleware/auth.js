// Middleware simples de autenticação baseado em sessão
// Em produção, use JWT ou outra solução mais robusta

const sessions = new Map(); // Armazena sessões em memória (use Redis em produção)

// Gera um token simples
function generateToken() {
  return Math.random().toString(36).substring(2) + Date.now().toString(36);
}

// Cria uma sessão para o usuário
function createSession(user) {
  const token = generateToken();
  const sessionData = {
    userId: user.id,
    nome: user.nome,
    email: user.email,
    tipo_usuario: user.tipo_usuario,
    createdAt: new Date()
  };
  
  sessions.set(token, sessionData);
  
  // Remove sessão após 24 horas
  setTimeout(() => {
    sessions.delete(token);
  }, 24 * 60 * 60 * 1000);
  
  return token;
}

// Verifica se o token é válido
function verifySession(token) {
  return sessions.get(token) || null;
}

// Remove uma sessão (logout)
function destroySession(token) {
  return sessions.delete(token);
}

// Middleware que verifica autenticação
function requireAuth(req, res, next) {
  const token = req.headers.authorization?.replace('Bearer ', '');
  
  if (!token) {
    return res.status(401).json({
      success: false,
      message: 'Token não fornecido. Faça login para continuar.'
    });
  }
  
  const session = verifySession(token);
  
  if (!session) {
    return res.status(401).json({
      success: false,
      message: 'Sessão inválida ou expirada. Faça login novamente.'
    });
  }
  
  // Adiciona dados do usuário na requisição
  req.user = session;
  next();
}

// Middleware que verifica se é administrador
function requireAdmin(req, res, next) {
  if (!req.user) {
    return res.status(401).json({
      success: false,
      message: 'Usuário não autenticado.'
    });
  }
  
  if (req.user.tipo_usuario !== 'admin') {
    return res.status(403).json({
      success: false,
      message: 'Acesso negado. Apenas administradores podem acessar este recurso.'
    });
  }
  
  next();
}

module.exports = {
  createSession,
  verifySession,
  destroySession,
  requireAuth,
  requireAdmin
};
