import { StyleSheet } from 'react-native';

const stylesdetailsScreen = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#000',
  },
  content: {
    flex: 1,
  },
  detailsCard: {
    backgroundColor: '#0D3B66',
    borderRadius: 12,
    padding: 30,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 6,
    elevation: 5,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#fff',
    marginBottom: 25,
  },
  detailsContainer: {
    gap: 15,
  },
  detailRow: {
    flexDirection: 'row',
    paddingVertical: 8,
  },
  label: {
    fontSize: 16,
    fontWeight: '600',
    color: '#fff',
    width: 150,
  },
  value: {
    fontSize: 16,
    color: '#E0E0E0',
    flex: 1,
  },
});

export default stylesdetailsScreen;