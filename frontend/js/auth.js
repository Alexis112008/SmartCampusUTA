const API = 'https://localhost:7001/api';

// ─── UTILIDADES ───────────────────────────────────────────

function mostrarError(id, mensaje) {
  const el = document.getElementById(id);
  if (!el) return;
  if (mensaje) {
    el.textContent = mensaje;
    el.classList.add('visible');
  } else {
    el.classList.remove('visible');
  }
}

function mostrarAlerta(id, visible) {
  const el = document.getElementById(id);
  if (!el) return;
  visible
    ? el.classList.add('visible')
    : el.classList.remove('visible');
}

function setLoading(btnId, spinnerId, textId, loading) {
  const btn = document.getElementById(btnId);
  const spinner = document.getElementById(spinnerId);
  const text = document.getElementById(textId);
  if (!btn || !spinner || !text) return;

  btn.disabled = loading;
  loading
    ? spinner.classList.add('visible')
    : spinner.classList.remove('visible');
  text.style.opacity = loading ? '0.6' : '1';
}

function togglePassword(inputId, btn) {
  const input = document.getElementById(inputId);
  if (input.type === 'password') {
    input.type = 'text';
    btn.textContent = '🙈';
  } else {
    input.type = 'password';
    btn.textContent = '👁️';
  }
}

async function hashPassword(password) {
  const msgBuffer = new TextEncoder().encode(password);
  const hashBuffer = await crypto.subtle.digest('SHA-256', msgBuffer);
  const hashArray = Array.from(new Uint8Array(hashBuffer));
  return hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
}

// ─── LOGIN ────────────────────────────────────────────────

const loginForm = document.getElementById('loginForm');
if (loginForm) {
  loginForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const email = document.getElementById('email').value.trim();
    const password = document.getElementById('password').value;
    let valido = true;

    mostrarAlerta('alertError', false);
    mostrarAlerta('alertSuccess', false);

    if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      mostrarError('emailError', 'Ingresa un correo válido');
      valido = false;
    } else {
      mostrarError('emailError', null);
    }

    if (!password) {
      mostrarError('passwordError', 'La contraseña es obligatoria');
      valido = false;
    } else {
      mostrarError('passwordError', null);
    }

    if (!valido) return;

    setLoading('btnLogin', 'loginSpinner', 'btnLoginText', true);

    try {
      const hash = await hashPassword(password);

      const response = await fetch(`${API}/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, passwordHash: hash })
      });

      if (response.ok) {
        const data = await response.json();

        localStorage.setItem('token', data.token);
        localStorage.setItem('usuario', JSON.stringify({
          id: data.idUsuario,
          nombre: data.nombre,
          rol: data.rol
        }));

        mostrarAlerta('alertSuccess', true);

        setTimeout(() => {
          const rol = data.rol;
          if (rol === 'Administrador') window.location.href = 'admin/dashboard.html';
          else if (rol === 'Docente')  window.location.href = 'docente/inicio.html';
          else if (rol === 'Estudiante') window.location.href = 'estudiante/inicio.html';
          else window.location.href = 'login.html';
        }, 1200);

      } else {
        const error = await response.json();
        document.getElementById('alertErrorMsg').textContent =
          error.mensaje || 'Credenciales incorrectas';
        mostrarAlerta('alertError', true);
      }

    } catch (err) {
      console.warn('Backend no disponible, usando modo demo');
      loginDemo(email, password);
    } finally {
      setLoading('btnLogin', 'loginSpinner', 'btnLoginText', false);
    }
  });
}

function loginDemo(email, password) {
  const usuarios = [
    { email: 'admin@uta.edu.ec',      password: 'admin123',  rol: 'Administrador', nombre: 'Admin UTA' },
    { email: 'secretaria@uta.edu.ec', password: 'secret123', rol: 'Secretaria',    nombre: 'María López' },
    { email: 'estudiante@uta.edu.ec', password: 'estud123',  rol: 'Estudiante',    nombre: 'Juan Pérez' },
  ];

  const usuario = usuarios.find(u => u.email === email && u.password === password);

  if (usuario) {
    localStorage.setItem('usuario', JSON.stringify(usuario));
    mostrarAlerta('alertSuccess', true);
    setTimeout(() => { window.location.href = 'dashboard.html'; }, 1200);
  } else {
    document.getElementById('alertErrorMsg').textContent =
      'Credenciales incorrectas. Modo demo: usa admin@uta.edu.ec / admin123';
    mostrarAlerta('alertError', true);
    setLoading('btnLogin', 'loginSpinner', 'btnLoginText', false);
  }
}

// ─── REGISTRO ─────────────────────────────────────────────

const rolSelect = document.getElementById('rol');
if (rolSelect) {
  rolSelect.addEventListener('change', () => {
    const campos = document.getElementById('camposEstudiante');
    if (campos) {
      campos.style.display = rolSelect.value === '3' ? 'block' : 'none';
    }
  });
}

const regPassword = document.getElementById('regPassword');
if (regPassword) {
  regPassword.addEventListener('input', () => {
    const val = regPassword.value;
    const bar = document.getElementById('strengthBar');
    const text = document.getElementById('strengthText');

    let score = 0;
    if (val.length >= 8)          score++;
    if (/[A-Z]/.test(val))        score++;
    if (/[0-9]/.test(val))        score++;
    if (/[^A-Za-z0-9]/.test(val)) score++;

    const niveles = [
      { pct: '0%',   color: '',                  label: '' },
      { pct: '25%',  color: '#dc3545',            label: 'Muy débil' },
      { pct: '50%',  color: '#fd7e14',            label: 'Débil' },
      { pct: '75%',  color: '#ffc107',            label: 'Aceptable' },
      { pct: '100%', color: 'var(--uta-success)', label: 'Fuerte ✓' },
    ];

    bar.style.width     = niveles[score].pct;
    bar.style.background = niveles[score].color;
    text.textContent    = niveles[score].label;
    text.style.color    = niveles[score].color;
  });
}

function irAlPaso2() {
  const nombre = document.getElementById('nombre')?.value.trim();
  const cedula = document.getElementById('cedula')?.value.trim();
  const rol    = document.getElementById('rol')?.value;
  let valido = true;

  if (!nombre) {
    mostrarError('nombreError', 'El nombre es obligatorio');
    valido = false;
  } else { mostrarError('nombreError', null); }

  if (!cedula || !/^\d{10}$/.test(cedula)) {
    mostrarError('cedulaError', 'Ingresa una cédula de 10 dígitos');
    valido = false;
  } else { mostrarError('cedulaError', null); }

  if (!rol) {
    mostrarError('rolError', 'Selecciona un tipo de usuario');
    valido = false;
  } else { mostrarError('rolError', null); }

  if (!valido) return;

  document.getElementById('step1').style.display = 'none';
  document.getElementById('step2').style.display = 'block';
  document.getElementById('step1Indicator').classList.remove('active');
  document.getElementById('step1Indicator').classList.add('done');
  document.getElementById('step1Indicator').querySelector('.step-circle').textContent = '✓';
  document.getElementById('step2Indicator').classList.add('active');
  document.getElementById('line1').classList.add('done');
}

function irAlPaso1() {
  document.getElementById('step2').style.display = 'none';
  document.getElementById('step1').style.display = 'block';
  document.getElementById('step2Indicator').classList.remove('active');
  document.getElementById('step1Indicator').classList.remove('done');
  document.getElementById('step1Indicator').classList.add('active');
  document.getElementById('step1Indicator').querySelector('.step-circle').textContent = '1';
  document.getElementById('line1').classList.remove('done');
}

const registerForm = document.getElementById('registerForm');
if (registerForm) {
  registerForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const email    = document.getElementById('regEmail')?.value.trim();
    const password = document.getElementById('regPassword')?.value;
    const confirm  = document.getElementById('confirmPassword')?.value;
    let valido = true;

    mostrarAlerta('regAlertError', false);

    if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      mostrarError('regEmailError', 'Ingresa un correo válido');
      valido = false;
    } else { mostrarError('regEmailError', null); }

    if (!password || password.length < 8) {
      mostrarError('regPasswordError', 'La contraseña debe tener al menos 8 caracteres');
      valido = false;
    } else { mostrarError('regPasswordError', null); }

    if (password !== confirm) {
      mostrarError('confirmError', 'Las contraseñas no coinciden');
      valido = false;
    } else { mostrarError('confirmError', null); }

    if (!valido) return;

    setLoading('btnRegister', 'registerSpinner', 'btnRegisterText', true);

    try {
      const hash = await hashPassword(password);

      const body = {
        nombre:   document.getElementById('nombre')?.value.trim(),
        email,
        passwordHash: hash,
        idRol:    parseInt(document.getElementById('rol')?.value),
        cedula:   document.getElementById('cedula')?.value.trim(),
        carrera:  document.getElementById('carrera')?.value || null,
        semestre: parseInt(document.getElementById('semestre')?.value) || 0
      };

      const response = await fetch(`${API}/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
      });

      if (response.ok) {
        mostrarAlerta('regAlertSuccess', true);
        setTimeout(() => { window.location.href = 'login.html'; }, 2000);
      } else {
        const error = await response.json();
        document.getElementById('regAlertErrorMsg').textContent =
          error.mensaje || 'Error al registrar usuario';
        mostrarAlerta('regAlertError', true);
      }

    } catch (err) {
      mostrarAlerta('regAlertSuccess', true);
      setTimeout(() => { window.location.href = 'login.html'; }, 2000);
    } finally {
      setLoading('btnRegister', 'registerSpinner', 'btnRegisterText', false);
    }
  });
}

// ─── RECUPERAR CONTRASEÑA ─────────────────────────────────

let emailRecuperacion = '';

const forgotForm = document.getElementById('forgotForm');
if (forgotForm) {
  forgotForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const email = document.getElementById('fpEmail')?.value.trim();
    mostrarError('fpEmailError', null);

    if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      mostrarError('fpEmailError', 'Ingresa un correo válido');
      return;
    }

    setLoading('btnForgot', 'forgotSpinner', 'btnForgotText', true);

    try {
      await fetch(`${API}/auth/solicitar-recuperacion`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email })
      });
    } catch (err) {
      console.warn('Backend no disponible, continuando en modo demo');
    } finally {
      // Siempre avanzamos a la vista del código (no revelamos si el email existe)
      emailRecuperacion = email;
      document.getElementById('sentToEmail').textContent = email;
      document.getElementById('viewEmail').style.display   = 'none';
      document.getElementById('viewConfirm').style.display = 'block';
      setLoading('btnForgot', 'forgotSpinner', 'btnForgotText', false);
    }
  });
}

async function reenviarCorreo() {
  if (!emailRecuperacion) return;

  const btn = document.getElementById('btnReenviar');
  btn.disabled = true;
  btn.textContent = 'Enviando...';

  try {
    await fetch(`${API}/auth/solicitar-recuperacion`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: emailRecuperacion })
    });
    btn.textContent = 'Reenviado ✓';
  } catch {
    btn.textContent = 'Error al reenviar';
  }

  setTimeout(() => {
    btn.disabled = false;
    btn.textContent = 'Reenviar código';
  }, 30000);
}

async function verificarCodigo() {
  const token = document.getElementById('tokenInput')?.value.trim();
  mostrarError('tokenError', null);

  if (!token || token.length !== 6) {
    mostrarError('tokenError', 'Ingresa el código de 6 dígitos');
    return;
  }

  setLoading('btnVerificar', 'verificarSpinner', 'btnVerificarText', true);

  try {
    const res = await fetch(`${API}/auth/verificar-token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token })
    });

    if (res.ok) {
      document.getElementById('viewConfirm').style.display  = 'none';
      document.getElementById('viewPassword').style.display = 'block';
    } else {
      const data = await res.json();
      mostrarError('tokenError', data.mensaje || 'Código inválido o expirado');
    }

  } catch (err) {
    // Modo demo: cualquier código de 6 dígitos pasa
    document.getElementById('viewConfirm').style.display  = 'none';
    document.getElementById('viewPassword').style.display = 'block';
  } finally {
    setLoading('btnVerificar', 'verificarSpinner', 'btnVerificarText', false);
  }
}

async function cambiarPassword() {
  const token     = document.getElementById('tokenInput')?.value.trim();
  const nuevaPass = document.getElementById('nuevaPass')?.value;
  const confirma  = document.getElementById('confirmaPass')?.value;

  mostrarError('nuevaPassError', null);
  mostrarError('confirmaPassError', null);

  let valido = true;

  if (!nuevaPass || nuevaPass.length < 8) {
    mostrarError('nuevaPassError', 'Mínimo 8 caracteres');
    valido = false;
  }

  if (nuevaPass !== confirma) {
    mostrarError('confirmaPassError', 'Las contraseñas no coinciden');
    valido = false;
  }

  if (!valido) return;

  setLoading('btnCambiar', 'cambiarSpinner', 'btnCambiarText', true);

  try {
    const hash = await hashPassword(nuevaPass);

    const res = await fetch(`${API}/auth/cambiar-password`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token, nuevaPassword: hash })
    });

    if (res.ok) {
      document.getElementById('viewPassword').style.display = 'none';
      document.getElementById('viewExito').style.display    = 'block';
      setTimeout(() => { window.location.href = 'login.html'; }, 3000);
    } else {
      const data = await res.json();
      document.getElementById('passAlertErrorMsg').textContent =
        data.mensaje || 'Error al cambiar la contraseña';
      document.getElementById('passAlertError').style.display = 'flex';
    }

  } catch (err) {
    // Modo demo
    document.getElementById('viewPassword').style.display = 'none';
    document.getElementById('viewExito').style.display    = 'block';
    setTimeout(() => { window.location.href = 'login.html'; }, 3000);
  } finally {
    setLoading('btnCambiar', 'cambiarSpinner', 'btnCambiarText', false);
  }
}