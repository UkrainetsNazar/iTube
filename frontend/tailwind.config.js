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
    },
  },
  plugins: [],
};
