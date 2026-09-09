/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  theme: {
    extend: {
      colors: {
        ink: {
          DEFAULT: '#14161B',
          soft: '#1B1E25',
        },
        surface: {
          DEFAULT: '#1D2027',
          hover: '#262A33',
          raised: '#23262E',
        },
        border: {
          DEFAULT: '#2C3038',
          soft: '#22252C',
        },
        paper: {
          DEFAULT: '#F2F1ED',
          dim: '#9A9CA5',
          faint: '#6B6D76',
        },
        signal: {
          DEFAULT: '#F2A93B',
          hover: '#FFBE5C',
        },
        moss: {
          DEFAULT: '#3ECF8E',
          hover: '#5CDBA1',
        },
        danger: {
          DEFAULT: '#E5484D',
          hover: '#F16469',
        },
      },
      fontFamily: {
        display: ['"Space Grotesk"', 'sans-serif'],
        sans: ['Inter', 'sans-serif'],
      },
      borderRadius: {
        card: '10px',
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
        fadeInUp: {
          '0%': { opacity: '0', transform: 'translateY(6px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
        scaleIn: {
          '0%': { opacity: '0', transform: 'scale(0.96)' },
          '100%': { opacity: '1', transform: 'scale(1)' },
        },
        slideInRight: {
          '0%': { opacity: '0', transform: 'translateX(12px)' },
          '100%': { opacity: '1', transform: 'translateX(0)' },
        },
      },
      animation: {
        'fade-in': 'fadeIn 0.25s ease-out both',
        'fade-in-up': 'fadeInUp 0.3s ease-out both',
        'scale-in': 'scaleIn 0.18s ease-out both',
        'slide-in-right': 'slideInRight 0.22s ease-out both',
      },
      transitionTimingFunction: {
        smooth: 'cubic-bezier(0.22, 1, 0.36, 1)',
      },
    },
  },
  plugins: [],
};
