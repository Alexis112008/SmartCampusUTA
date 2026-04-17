const API = 'https://localhost:7001/api';
// Cambia el puerto al que usa tu proyecto en Visual Studio

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

// Hash SHA-256 simple para el frontend
// En producción el backend siempre rehashea con BCrypt
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

    // Validaciones
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

        // Guardar sesión
        localStorage.setItem('token', data.token);
        localStorage.setItem('usuario', JSON.stringify({
          id: data.idUsuario,
          nombre: data.nombre,
          rol: data.rol
        }));

        mostrarAlerta('alertSuccess', true);

        // Redirigir según rol después de 1.2s
        setTimeout(() => {
          const rol = data.rol.toLowerCase();
          if (rol === 'administrador') {
            window.location.href = 'dashboard.html';
          } else if (rol === 'secretaria') {
            window.location.href = 'turnos.html';
          } else {
            window.location.href = 'inicio.html';
          }
        }, 1200);

      } else {
        const error = await response.json();
        document.getElementById('alertErrorMsg').textContent =
          error.mensaje || 'Credenciales incorrectas';
        mostrarAlerta('alertError', true);
      }

    } catch (err) {
      // Si el backend no responde, modo demo con datos quemados
      console.warn('Backend no disponible, usando modo demo');
      loginDemo(email, password);
    } finally {
      setLoading('btnLogin', 'loginSpinner', 'btnLoginText', false);
    }
  });
}

// Modo demo: funciona sin backend para la presentación
function loginDemo(email, password) {
  const usuarios = [
    { email: 'admin@uta.edu.ec',      password: 'admin123',  rol: 'Administrador', nombre: 'Admin UTA' },
    { email: 'secretaria@uta.edu.ec', password: 'secret123', rol: 'Secretaria',    nombre: 'María López' },
    { email: 'estudiante@uta.edu.ec', password: 'estud123',  rol: 'Estudiante',    nombre: 'Juan Pérez' },
  ];

  const usuario = usuarios.find(
    u => u.email === email && u.password === password
  );

  if (usuario) {
    localStorage.setItem('usuario', JSON.stringify(usuario));
    mostrarAlerta('alertSuccess', true);
    setTimeout(() => {
      window.location.href = 'dashboard.html';
    }, 1200);
  } else {
    document.getElementById('alertErrorMsg').textContent =
      'Credenciales incorrectas. Modo demo: usa admin@uta.edu.ec / admin123';
    mostrarAlerta('alertError', true);
    setLoading('btnLogin', 'loginSpinner', 'btnLoginText', false);
  }
}

// ─── REGISTRO ─────────────────────────────────────────────

// Mostrar campos de estudiante al seleccionar rol
const rolSelect = document.getElementById('rol');
if (rolSelect) {
  rolSelect.addEventListener('change', () => {
    const campos = document.getElementById('camposEstudiante');
    if (campos) {
      campos.style.display = rolSelect.value === '3' ? 'block' : 'none';
    }
  });
}

// Indicador de fortaleza de contraseña
const regPassword = document.getElementById('regPassword');
if (regPassword) {
  regPassword.addEventListener('input', () => {
    const val = regPassword.value;
    const bar = document.getElementById('strengthBar');
    const text = document.getElementById('strengthText');

    let score = 0;
    if (val.length >= 8)  score++;
    if (/[A-Z]/.test(val)) score++;
    if (/[0-9]/.test(val)) score++;
    if (/[^A-Za-z0-9]/.test(val)) score++;

    const niveles = [
      { pct: '0%',   color: '',                  label: '' },
      { pct: '25%',  color: '#dc3545',            label: 'Muy débil' },
      { pct: '50%',  color: '#fd7e14',            label: 'Débil' },
      { pct: '75%',  color: '#ffc107',            label: 'Aceptable' },
      { pct: '100%', color: 'var(--uta-success)', label: 'Fuerte ✓' },
    ];

    bar.style.width = niveles[score].pct;
    bar.style.background = niveles[score].color;
    text.textContent = niveles[score].label;
    text.style.color = niveles[score].color;
  });
}

function irAlPaso2() {
  const nombre = document.getElementById('nombre')?.value.trim();
  const cedula = document.getElementById('cedula')?.value.trim();
  const rol = document.getElementById('rol')?.value;
  let valido = true;

  if (!nombre) {
    mostrarError('nombreError', 'El nombre es obligatorio');
    valido = false;
  } else {
    mostrarError('nombreError', null);
  }

  if (!cedula || !/^\d{10}$/.test(cedula)) {
    mostrarError('cedulaError', 'Ingresa una cédula de 10 dígitos');
    valido = false;
  } else {
    mostrarError('cedulaError', null);
  }

  if (!rol) {
    mostrarError('rolError', 'Selecciona un tipo de usuario');
    valido = false;
  } else {
    mostrarError('rolError', null);
  }

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

    const email = document.getElementById('regEmail')?.value.trim();
    const password = document.getElementById('regPassword')?.value;
    const confirm = document.getElementById('confirmPassword')?.value;
    let valido = true;

    mostrarAlerta('regAlertError', false);

    if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      mostrarError('regEmailError', 'Ingresa un correo válido');
      valido = false;
    } else {
      mostrarError('regEmailError', null);
    }

    if (!password || password.length < 8) {
      mostrarError('regPasswordError',
        'La contraseña debe tener al menos 8 caracteres');
      valido = false;
    } else {
      mostrarError('regPasswordError', null);
    }

    if (password !== confirm) {
      mostrarError('confirmError', 'Las contraseñas no coinciden');
      valido = false;
    } else {
      mostrarError('confirmError', null);
    }

    if (!valido) return;

    setLoading('btnRegister', 'registerSpinner', 'btnRegisterText', true);

    try {
      const hash = await hashPassword(password);

      const body = {
        nombre: document.getElementById('nombre')?.value.trim(),
        email,
        passwordHash: hash,
        idRol: parseInt(document.getElementById('rol')?.value),
        cedula: document.getElementById('cedula')?.value.trim(),
        carrera: document.getElementById('carrera')?.value || null,
        semestre: parseInt(document.getElementById('semestre')?.value) || 0
      };

      const response = await fetch(`${API}/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
      });

      if (response.ok) {
        mostrarAlerta('regAlertSuccess', true);
        setTimeout(() => {
          window.location.href = 'login.html';
        }, 2000);
      } else {
        const error = await response.json();
        document.getElementById('regAlertErrorMsg').textContent =
          error.mensaje || 'Error al registrar usuario';
        mostrarAlerta('regAlertError', true);
      }

    } catch (err) {
      // Modo demo
      mostrarAlerta('regAlertSuccess', true);
      setTimeout(() => { window.location.href = 'login.html'; }, 2000);
    } finally {
      setLoading('btnRegister', 'registerSpinner', 'btnRegisterText', false);
    }
  });
}

// ─── RECUPERAR CONTRASEÑA ─────────────────────────────────

const forgotForm = document.getElementById('forgotForm');
if (forgotForm) {
  forgotForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const email = document.getElementById('fpEmail')?.value.trim();
    mostrarAlerta('fpAlertError', false);

    if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      mostrarError('fpEmailError', 'Ingresa un correo válido');
      return;
    }
    mostrarError('fpEmailError', null);

    setLoading('btnForgot', 'forgotSpinner', 'btnForgotText', true);

    // Simulación: en producción llama al endpoint real
    await new Promise(r => setTimeout(r, 1500));

    document.getElementById('sentToEmail').textContent = email;
    document.getElementById('viewEmail').style.display = 'none';
    document.getElementById('viewConfirm').style.display = 'block';

    setLoading('btnForgot', 'forgotSpinner', 'btnForgotText', false);
  });
}

function reenviarCorreo() {
  const btn = document.getElementById('btnReenviar');
  btn.disabled = true;
  btn.textContent = 'Reenviado ✓';
  setTimeout(() => {
    btn.disabled = false;
    btn.textContent = 'Reenviar correo';
  }, 30000);
}