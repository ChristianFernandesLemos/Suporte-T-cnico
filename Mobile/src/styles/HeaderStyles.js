import { StyleSheet, Platform, StatusBar } from 'react-native';

const Headerstyles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: 20,
    // Aumentamos o padding superior para baixar o header
    paddingTop: Platform.OS === 'android' ? (StatusBar.currentHeight || 0) + 20 : 50, 
    paddingBottom: 15,
    backgroundColor: '#fff',
    borderBottomWidth: 1,
    borderBottomColor: '#e0e0e0',
    // Adiciona uma sombra leve para separar do conteúdo
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  leftSection: {
    flex: 1,
  },
  backButton: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  backText: {
    marginLeft: 5,
    fontSize: 16,
    color: '#000',
  },
  logo: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#000',
  },
  logoHighlight: {
    color: '#00BFFF',
  },
  title: {
    fontSize: 18,
    fontWeight: '600',
    flex: 2,
    textAlign: 'center',
    color: '#333',
  },
  profileButton: {
    padding: 5,
  },
});

export default Headerstyles;